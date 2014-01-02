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

namespace SmartPlan.Controllers
{
    public class AccountController : Controller
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
            return View();
        }
    }
}
