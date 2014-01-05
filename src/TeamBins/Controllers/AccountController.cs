using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Planner.DataAccess;
using SmartPlan.Hubs;
using SmartPlan.Services;
using SmartPlan.ViewModels;

using TechiesWeb.TeamBins.ViewModels;
using SmartPlan.DataAccess;
using TechiesWeb.TeamBins.Infrastructure;


namespace Planner.Controllers
{
    public class AccountController : BaseController
    {
            IRepositary repo;

            public AccountController()
        {
            repo = new Repositary();
        }


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Join()
        {
            return View(new AccountSignupVM());
        }



        [HttpPost]
        public ActionResult Join(AccountSignupVM model)
        {
            if (ModelState.IsValid)
            {
                
                var user = repo.GetUser(model.Email);
                if (user == null)
                {
                    var newUser = new User { EmailAddress = model.Email, FirstName=model.Name, Password=model.Password };
                   // SecurityService.SetNewPassword(newUser, model.Password);                  
                   var result = repo.SaveUser(newUser);
                    if (result.Status)
                    {
                        var team = new Team { Name = newUser.ID.ToString() };
                        var res = repo.SaveTeam(team);
                       /* var notificationVM = new NotificationVM { Title = "New User Joined", Message = model.Name + " joined" };
                        var context = GlobalHost.ConnectionManager.GetHubContext<UserHub>();
                        context.Clients.All.ShowNotificaion(notificationVM);*/

                        return RedirectToAction("AccountCreated");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Account already exists with this email address");
                }
            }
            return View(model);
        }
        public ActionResult AccountCreated()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View(new LoginVM { Email = "admin@team", Password = "admin" });
        }
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = repo.GetUser(model.Email);
                if (user != null)
                {
                    string hashed = SecurityService.GetPasswordHash(model.Password);
                  // var s= PasswordHash.ValidatePassword(model.Password,user.HA);
                   if (user.Password==model.Password)
                   {
                       return RedirectToAction("Index", "Dashboard");
                   }
                }
            }
            ModelState.AddModelError("", "Username/Password is incorrect!");
            return View(model);
        }

        public ActionResult reset(string id)
        {
            var vm = new ResetPasswordVM();
            return View(vm);
        }

        public ActionResult forgotPassword()
        {
            return View(new ForgotPasswordVM());
        }
        [HttpPost]
        public ActionResult forgotpassword(ForgotPasswordVM model)
        {
            return View();
        }
        public ActionResult Profile()
        {
            
            var user = repo.GetUser(UserID);
            if(user!=null)
            {
                var vm = new EditProfileVM { Name = user.FirstName, Email = user.EmailAddress };
                return View(vm);
            }
            return View("NotFound");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult EditProfile()
        {
            var user = repo.GetUser(UserID);
            if (user != null)
            {
                var vm = new EditProfileVM { Name = user.FirstName, Email = user.EmailAddress };
                return View(vm);
            }
            return View("NotFound");
        }

        [HttpPost]
        public ActionResult EditProfile(EditProfileVM model)
        {
            if(ModelState.IsValid)
            {
                var user = repo.GetUser(UserID);
                if(user!=null)
                {
                    user.FirstName = model.Name;
                    var result = repo.SaveUser(user);
                    if(result.Status)
                    {
                        var msg = new AlertMessageStore();
                        msg.AddMessage("success", "Profile updated successfully");
                        TempData["AlertMessages"] = msg;
                    }
                }
            }
            return View(model);
        }

        public ActionResult Password()
        {
            var vm = new ChangePasswordVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Password(ChangePasswordVM model)
        {
            if(ModelState.IsValid)
            {
                var user = repo.GetUser(UserID);
                if (user != null && user.Password==model.Password)
                {
                    user.Password = model.Password;
                    var result = repo.SaveUser(user);
                    if (result.Status)
                    {
                        var msg = new AlertMessageStore();
                        msg.AddMessage("success", "Password updated successfully");
                        TempData["AlertMessages"] = msg;
                    }
                }
            }
            return View(model);
        }
    }
}
