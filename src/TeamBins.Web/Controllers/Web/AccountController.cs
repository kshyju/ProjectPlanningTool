
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre;
using TeamBins.Services;
using TeamBins.Infrastrucutre.Services;


namespace TeamBins.Controllers.Web
{
    public class AccountController : BaseController
    {
        readonly IUserAccountManager _userAccountManager;
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly ITeamManager _teamManager;

        public AccountController(IUserAccountManager userAccountManager, IUserAuthHelper userSessionHelper, ITeamManager teamManager, IOptions<AppSettings> settings) : base(settings)
        {
            this._userAccountManager = userAccountManager;
            this._userSessionHelper = userSessionHelper;
            this._teamManager = teamManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM model)
        {
            tc.TrackEvent("Login attempt");
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userAccountManager.GetUser(model.Email);
                    if (user != null)
                    {
                        if (user.Password == model.Password)
                        {
                            await _userAccountManager.UpdateLastLoginTime(user.Id);

                            if (user.DefaultTeamId == null)
                            {
                                tc.TrackEvent("User with no default team!"+user.Id);
                                // This sould not happen! But if in case
                                var teams = await _userAccountManager.GetTeams(user.Id);
                                if (teams.Any())
                                {
                                    user.DefaultTeamId = teams.First().Id;
                                    await this._userAccountManager.SetDefaultTeam(user.Id, user.DefaultTeamId.Value);
                                }
                            }

                            this._userSessionHelper.SetUserIDToSession(user.Id, user.DefaultTeamId.Value, model.Email);
                            return RedirectToAction("index", "dashboard");
                        }
                    }
                }
                ModelState.AddModelError("", "Username/Password is incorrect!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Oops! Something went wrong :(");


            }
            return View(model);
        }

        public ActionResult Join(string returnurl = "")
        {
            tc.TrackEvent("Joining via join link");
            return View(new AccountSignupVM { ReturnUrl = returnurl });
        }

        [HttpPost]
        public async Task<ActionResult> Join(AccountSignupVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var accountExists = await _userAccountManager.GetUser(model.Email);
                    if (accountExists == null)
                    {
                        var newUser = new UserAccountDto
                        {
                            EmailAddress = model.Email,
                            Name = model.Name,
                            Password = model.Password
                        };
                        var userSession = await _userAccountManager.CreateAccount(newUser);

                        if (userSession.UserId > 0)
                        {
                            _userSessionHelper.SetUserIDToSession(userSession);
                        }

                        if (!String.IsNullOrEmpty(model.ReturnUrl))
                            return RedirectToAction("joinmyteam", "users", new { id = model.ReturnUrl });

                        return RedirectToAction("accountcreated");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Account already exists with this email address");
                    }
                }
            }
            catch (Exception ex)
            {
                tc.TrackException(ex);
            }
            return View(model);

        }

        public ActionResult AccountCreated()
        {
            return View();
        }

        public ActionResult Logout()
        {
            this._userSessionHelper.Logout();
            return RedirectToAction("login", "account");
        }
        public async Task<JsonResult> SwitchTeam(int id)
        {
            if (!_teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, id))
            {
                tc.TrackEvent("Trying to access some one else's team");
                return Json(new { Status = "Error", Message = "You do not belong to this team!" });
            }
               

            _userSessionHelper.SetTeamId(id);
            await _userAccountManager.SetDefaultTeam(_userSessionHelper.UserId, id);
            return Json(new { Status = "Success" });
        }

        #region Reset Password

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userAccountManager.GetUser(model.Email);
                if (user != null)
                {
                    await _userAccountManager.SavePasswordResetRequest(user);
                    return RedirectToAction("ForgotPasswordEmailSent");
                }
                ModelState.AddModelError(string.Empty, "No user account found for this email.");
            }
            return View(model);
        }

        public ActionResult Reset(string id)
        {
            var vm = new ResetPasswordVM();
            return View(vm);
        }

        public ActionResult ForgotPassword()
        {
            return View("forgotPassword", new ForgotPasswordVm());
        }

        public IActionResult ForgotPasswordEmailSent()
        {
            return View();
        }
        public async Task<IActionResult> ResetPassword(string id)
        {
            //coming from the password reset link received in email
            var passwordResetRequest = await _userAccountManager.GetPasswordResetRequest(id);
            if (passwordResetRequest != null)
            {
                return View(new ResetPasswordVM { ActivationCode = passwordResetRequest.ActivationCode });
            }
            return View("NotFound");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var passwordResetRequest = await _userAccountManager.GetPasswordResetRequest(model.ActivationCode);
                if (passwordResetRequest != null)
                {
                    await _userAccountManager.UpdatePassword(model.Password, passwordResetRequest.UserId);
                    // delete the request may be ?
                    return RedirectToAction("passwordupdated");
                }
                return View("NotFound");
            }
            return View(model);

        }
        public ActionResult PasswordUpdated()
        {
            return View();
        }

        #endregion Reset Password
    }
}