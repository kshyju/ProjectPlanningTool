using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins.Services;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBins.ViewModels;

namespace TechiesWeb.TeamBins.Controllers
{
    public class AccountController : BaseController
    {
        readonly IUserAccountManager accountManager;

        public AccountController(IUserAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        public AccountController(IRepositary repositary, IUserAccountManager accountManager) : base(repositary)
        {
            this.accountManager = accountManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Join(string returnurl = "")
        {
            return View(new AccountSignupVM { ReturnUrl = returnurl });
        }

        [HttpPost]
        public ActionResult Join(AccountSignupVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var accountExists = accountManager.DoesAccountExist(model.Email);
                    if (!accountExists)
                    {
                        var newUser = new UserAccountDto { EmailAddress = model.Email, Name = model.Name, Password = model.Password };
                        var userSession = accountManager.CreateUserAccount(newUser);

                        if (userSession.UserId > 0)
                        {
                            SetUserIDToSession(userSession);
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
                log.Error(ex);
            }
            return View(model);
        }

        public ActionResult AccountCreated()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View("Login", new LoginVM());
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = accountManager.GetUser(model.Email);
                    if (user != null)
                    {
                        if (user.Password == model.Password)
                        {
                            await accountManager.SaveLastLoginAsync(user.Id);
                            int userDefaultTeamId = user.DefaultTeamId ?? 0;


                            //var claims = new[] {
                            //        new Claim(ClaimTypes.Name, user.Name),
                            //        new Claim(ClaimTypes.Email, user.EmailAddress)
                            // };
                            //var identity = new ClaimsIdentity(claims,"teambins");
                            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                            //Thread.CurrentPrincipal = principal;
                            //var context = Request.GetOwinContext();
                            //var authManager = context.Authentication;

                            //authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                            SetUserIDToSession(user.Id, userDefaultTeamId, user.Name);

                            return RedirectToAction("index", "dashboard");
                        }
                    }
                }
                ModelState.AddModelError("", "Username/Password is incorrect!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Oops! Something went wrong :(");
                log.Error(ex);
            }
            return View(model);
        }

        public ActionResult NotificationSettings()
        {
            var vm = new UserEmailNotificationSettingsVM { TeamID = TeamID };
            var userSubscriptions = repo.GetUser(UserID).UserNotificationSubscriptions.ToList();

            var notificationTypes = repo.GetNotificationTypes().ToList();
            foreach (var item in notificationTypes)
            {
                var emailSubscription = new EmailSubscriptionVM { NotificationTypeID = item.ID, Name = item.Name };
                emailSubscription.IsSelected = userSubscriptions.Any(s => s.UserID == UserID && s.TeamID == TeamID && s.NotificationTypeID == item.ID && s.Subscribed == true);
                vm.EmailSubscriptions.Add(emailSubscription);
            }

            return View(vm);
        }
        [HttpPost]
        public ActionResult NotificationSettings(UserEmailNotificationSettingsVM model)
        {
            try
            {
                foreach (var setting in model.EmailSubscriptions)
                {
                    var userNotification = new UserNotificationSubscription { TeamID = TeamID, UserID = UserID };
                    userNotification.Subscribed = setting.IsSelected;
                    userNotification.ModifiedDate = DateTime.UtcNow;
                    userNotification.NotificationTypeID = setting.NotificationTypeID;
                    repo.SaveUserNotificationSubscription(userNotification);
                }
                var msg = new AlertMessageStore();
                msg.AddMessage("success", "Notification Settings updated successfully");
                TempData["AlertMessages"] = msg;
                return RedirectToAction("NotificationSettings");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return View("Error");
            }
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
        public ActionResult forgotPasswordEmailSent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult forgotpassword(ForgotPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = accountManager.ProcessPasswordRecovery(model.Email);
                    if (result != null)
                    {
                        return RedirectToAction("forgotpasswordemailsent");
                    }
                    ModelState.AddModelError("", "We do not see an account with that email address!");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error in processing the your request");
            }
            return View(model);
        }

        public ActionResult ResetPassword(string id)
        {
            var resetPasswordRequest = accountManager.GetResetPaswordRequest(id);

            if (resetPasswordRequest != null)
            {
                return View(new ResetPasswordVM { ActivationCode = resetPasswordRequest.ActivationCode });
            }
            return View("NotFound");
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var updatePasswordResult = accountManager.ResetPassword(model.ActivationCode, model.Password);
                if (updatePasswordResult)
                {
                    return RedirectToAction("passwordupdated");
                }
            }
            return View(model);

        }
        public ActionResult PasswordUpdated()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("login", "account");
        }


    }
}
