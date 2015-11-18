using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TechiesWeb.TeamBins.Infrastructure;

namespace TeamBins.Controllers
{
    public class SettingsController : Controller
    {
        readonly IUserAccountManager userAccountManager;
        public SettingsController(IUserAccountManager userAccountManager)
        {
            this.userAccountManager = userAccountManager;
        }

        // GET: Settings
        public ActionResult Index()
        {
            var vm = new SettingsVm
            {
                Profile = userAccountManager.GetUserProfile(),
                NotificationSettings = userAccountManager.GetNotificationSettings(),
                IssueSettings = userAccountManager.GetIssueSettingsForUser()
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditProfile(EditProfileVm model)
        {
            if (ModelState.IsValid)
            {
                userAccountManager.UpdateProfile(model);
                var msg = new AlertMessageStore();
                msg.AddMessage("success", "Profile updated successfully");
                TempData["AlertMessages"] = msg;
                return RedirectToAction("Index", "Settings");

            }
            //TO DO : Redirect to the Tab ?
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Password(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                userAccountManager.UpdatePassword(model);

                var msg = new AlertMessageStore();
                msg.AddMessage("success", "Password updated successfully");
                TempData["AlertMessages"] = msg;
            }
            //TO DO : Redirect to the Tab ?
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Settings(DefaultIssueSettings model)
        {
            if (ModelState.IsValid)
            {
                 userAccountManager.SaveDefaultProjectForTeam(model.SelectedProject);
                
                    var msg = new AlertMessageStore();
                    msg.AddMessage("success", "Settings updated successfully");
                    TempData["AlertMessages"] = msg;
                    return RedirectToAction("settings");
                
            }
            //
          //  model.Projects = GetProjectListItem();
            return RedirectToAction("Index");
        }

    }


}
