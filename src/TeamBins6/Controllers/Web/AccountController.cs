////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Security.Claims;
////using System.Threading.Tasks;
////using Microsoft.AspNet.Http.Authentication;
////using Microsoft.AspNet.Mvc;
////using TeamBins.Services;

////// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860


//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Mvc;
//using TeamBins.Common;
//using TeamBins.Common.ViewModels;
//using TeamBins.Services;
//using TeamBins6.Infrastrucutre.Services;

//namespace TeamBins6.Controllers.Web
//{
//    public class AccountController : Controller
//    {
//        IUserAccountManager userAccountManager;
//        private IUserSessionHelper userSessionHelper;

//        public AccountController(IUserAccountManager userAccountManager, IUserSessionHelper userSessionHelper)
//        {
//            this.userAccountManager = userAccountManager;
//            this.userSessionHelper = userSessionHelper;
//        }

//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<ActionResult> Login(LoginVM model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    var user = await userAccountManager.GetUser(model.Email);
//                    if (user != null)
//                    {
//                        var appUser = new AppUser {UserName = user.EmailAddress, Id = user.Id.ToString()};
//                        if (user.Password == model.Password)
//                        {
//                            if (user.DefaultTeamId == null)
//                            {
//                                // This sould not happen! But if in case
//                               var teams= await userAccountManager.GetTeams(user.Id);
//                                if (teams.Any())
//                                {
//                                    user.DefaultTeamId = teams.First().Id;
//                                    await this.userAccountManager.SetDefaultTeam(user.Id, user.DefaultTeamId.Value);
//                                }
//                            }
                            
//                            this.userSessionHelper.SetUserIDToSession(user.Id, user.DefaultTeamId.Value);
//                            return RedirectToAction("index", "dashboard");
//                        }
//                    }
//                }
//                ModelState.AddModelError("", "Username/Password is incorrect!");
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", "Oops! Something went wrong :(");

//            }
//            return View(model);
//        }


//        public ActionResult reset(string id)
//        {
//            var vm = new ResetPasswordVM();
//            return View(vm);
//        }

//        public ActionResult forgotPassword()
//        {
//            return View("forgotPassword", new ForgotPasswordVM());
//        }

//        public ActionResult Join(string returnurl = "")
//        {
//            return View(new AccountSignupVM {ReturnUrl = returnurl});
//        }

//        [HttpPost]
//        public async Task<ActionResult> Join(AccountSignupVM model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    var accountExists = await userAccountManager.GetUser(model.Email);
//                    if (accountExists == null)
//                    {
//                        var newUser = new UserAccountDto
//                        {
//                            EmailAddress = model.Email,
//                            Name = model.Name,
//                            Password = model.Password
//                        };
//                        var userSession = await userAccountManager.CreateAccount(newUser);

//                        if (userSession.UserId > 0)
//                        {
//                            userSessionHelper.SetUserIDToSession(userSession);
//                        }

//                        if (!String.IsNullOrEmpty(model.ReturnUrl))
//                            return RedirectToAction("joinmyteam", "users", new { id = model.ReturnUrl });

//                        return RedirectToAction("accountcreated");

//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", "Account already exists with this email address");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//            return View(model);

//        }

//        public ActionResult AccountCreated()
//        {
//            return View();
//        }

//        public ActionResult Logout()
//        {
//            this.userSessionHelper.Logout();
//            return RedirectToAction("login", "account");
//        }
//    }
//}

////    {
////        readonly IUserAccountManager accountManager;
////        private UserManager<AppUser> um;
////        public AccountController(IUserAccountManager accountManager)
////        {
////            this.accountManager = accountManager;


////        }

////        public AccountController(IRepositary repositary, IUserAccountManager accountManager) : base(repositary)
////        {
////            this.accountManager = accountManager;
////        }

////        public ActionResult Index()
////        {
////            return RedirectToAction("Login");
////        }


////        }



////        public ActionResult Login()
////        {




////            return View("Login", new LoginVM());
////        }
////        private IAuthenticationManager AuthenticationManager
////        {
////            get
////            {
////                return HttpContext.GetOwinContext().Authentication;
////            }
////        }
////        private async Task SignInAsync(AppUser user, bool isPersistent)
////        {
////            um = new UserManager<AppUser>(new UserStore());

////            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
////            var identity = await um.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

////            //identity.Claims =new 
////            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

////            //   var identity = new ClaimsIdentity(claims,DefaultAuthenticationTypes.ApplicationCookie);
////            //ClaimsPrincipal principal = new ClaimsPrincipal(identity);
////            //Thread.CurrentPrincipal = principal;
////            //var context = Request.GetOwinContext();
////            //var authManager = context.Authentication;

////            //authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

////            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
////            //var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
////            //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);


////        }

////  
////        public ActionResult NotificationSettings()
////        {
////            var vm = new UserEmailNotificationSettingsVM { TeamId = TeamId };
////            var userSubscriptions = repo.GetUser(UserID).UserNotificationSubscriptions.ToList();

////            var notificationTypes = repo.GetNotificationTypes().ToList();
////            foreach (var item in notificationTypes)
////            {
////                var emailSubscription = new EmailSubscriptionVM { NotificationTypeID = item.ID, Name = item.Name };
////                emailSubscription.IsSelected = userSubscriptions.Any(s => s.UserID == UserID && s.TeamId == TeamId && s.NotificationTypeID == item.ID && s.Subscribed == true);
////                vm.EmailSubscriptions.Add(emailSubscription);
////            }

////            return View(vm);
////        }
////        [HttpPost]
////        public ActionResult NotificationSettings(UserEmailNotificationSettingsVM model)
////        {
////            try
////            {
////                foreach (var setting in model.EmailSubscriptions)
////                {
////                    var userNotification = new UserNotificationSubscription { TeamId = TeamId, UserID = UserID };
////                    userNotification.Subscribed = setting.IsSelected;
////                    userNotification.ModifiedDate = DateTime.UtcNow;
////                    userNotification.NotificationTypeID = setting.NotificationTypeID;
////                    repo.SaveUserNotificationSubscription(userNotification);
////                }
////                var msg = new AlertMessageStore();
////                msg.AddMessage("success", "Notification Settings updated successfully");
////                TempData["AlertMessages"] = msg;
////                return RedirectToAction("NotificationSettings");
////            }
////            catch (Exception ex)
////            {
////                log.Error(ex);
////                return View("Error");
////            }
////        }


////        public ActionResult forgotPasswordEmailSent()
////        {
////            return View();
////        }
////        [HttpPost]
////        public ActionResult forgotpassword(ForgotPasswordVM model)
////        {
////            try
////            {
////                if (ModelState.IsValid)
////                {
////                    var result = accountManager.ProcessPasswordRecovery(model.Email);
////                    if (result != null)
////                    {
////                        return RedirectToAction("forgotpasswordemailsent");
////                    }
////                    ModelState.AddModelError("", "We do not see an account with that email address!");
////                    return View();
////                }
////            }
////            catch (Exception ex)
////            {
////                ModelState.AddModelError("", "Error in processing the your request");
////            }
////            return View(model);
////        }

////        public ActionResult ResetPassword(string id)
////        {
////            var resetPasswordRequest = accountManager.GetResetPaswordRequest(id);

////            if (resetPasswordRequest != null)
////            {
////                return View(new ResetPasswordVM { ActivationCode = resetPasswordRequest.ActivationCode });
////            }
////            return View("NotFound");
////        }

////        [HttpPost]
////        public ActionResult ResetPassword(ResetPasswordVM model)
////        {
////            if (ModelState.IsValid)
////            {
////                var updatePasswordResult = accountManager.ResetPassword(model.ActivationCode, model.Password);
////                if (updatePasswordResult)
////                {
////                    return RedirectToAction("passwordupdated");
////                }
////            }
////            return View(model);

////        }
////        public ActionResult PasswordUpdated()
////        {
////            return View();
////        }




////    }
////}
