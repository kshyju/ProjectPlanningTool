
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins.Infrastrucutre.Services;


namespace TeamBins.Controllers.Web
{
    public class AccountController : BaseController
    {
        readonly IUserAccountManager userAccountManager;
        private readonly IUserAuthHelper userSessionHelper;
        private readonly ITeamManager teamManager;

        public AccountController(IUserAccountManager userAccountManager, IUserAuthHelper userSessionHelper, ITeamManager teamManager)
        {
            this.userAccountManager = userAccountManager;
            this.userSessionHelper = userSessionHelper;
            this.teamManager = teamManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userAccountManager.GetUser(model.Email);
                    if (user != null)
                    {
                        var appUser = new AppUser { UserName = user.EmailAddress, Id = user.Id.ToString() };
                        if (user.Password == model.Password)
                        {
                            await userAccountManager.UpdateLastLoginTime(user.Id);

                            if (user.DefaultTeamId == null)
                            {
                                // This sould not happen! But if in case
                                var teams = await userAccountManager.GetTeams(user.Id);
                                if (teams.Any())
                                {
                                    user.DefaultTeamId = teams.First().Id;
                                    await this.userAccountManager.SetDefaultTeam(user.Id, user.DefaultTeamId.Value);
                                }
                            }

                            this.userSessionHelper.SetUserIDToSession(user.Id, user.DefaultTeamId.Value);
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

        public ActionResult reset(string id)
        {
            var vm = new ResetPasswordVM();
            return View(vm);
        }

        public ActionResult forgotPassword()
        {
            return View("forgotPassword", new ForgotPasswordVM());
        }

        public ActionResult Join(string returnurl = "")
        {
            return View(new AccountSignupVM { ReturnUrl = returnurl });
        }

        [HttpPost]
        public async Task<ActionResult> Join(AccountSignupVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var accountExists = await userAccountManager.GetUser(model.Email);
                    if (accountExists == null)
                    {
                        var newUser = new UserAccountDto
                        {
                            EmailAddress = model.Email,
                            Name = model.Name,
                            Password = model.Password
                        };
                        var userSession = await userAccountManager.CreateAccount(newUser);

                        if (userSession.UserId > 0)
                        {
                            userSessionHelper.SetUserIDToSession(userSession);
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

            }
            return View(model);

        }

        public ActionResult AccountCreated()
        {
            return View();
        }

        public ActionResult Logout()
        {
            this.userSessionHelper.Logout();
            return RedirectToAction("login", "account");
        }
        public async Task<JsonResult> SwitchTeam(int id)
        {
            if (!teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId, id))
                return Json(new { Status = "Error", Message = "You do not belong to this team!" });

            userSessionHelper.SetTeamId(id);
            await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, id);
            return Json(new { Status = "Success" });
        }
    }
}