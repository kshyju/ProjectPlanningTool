
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Controllers;
using TeamBins6.Infrastrucutre;


namespace TeamBins.Controllers
{
    public class SettingsController : BaseController
    {
        readonly IUserAccountManager userAccountManager;
        public SettingsController(IUserAccountManager userAccountManager)
        {
            this.userAccountManager = userAccountManager;
        }

        // GET: Settings
        public async Task<ActionResult> Index()
        {
            var vm = new SettingsVm
            {
                Profile = await userAccountManager.GetUserProfile(),
                NotificationSettings =await userAccountManager.GetNotificationSettings(),
                IssueSettings = await userAccountManager.GetIssueSettingsForUser()
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> Notifications(UserEmailNotificationSettingsVM model)
        {
            await userAccountManager.SaveNotificationSettings(model);
            SetMessage(MessageType.Success,"Settings updated successfully");
            return RedirectToAction("Index", "Settings");
        }
        [HttpPost]
        public async Task<ActionResult> EditProfile(EditProfileVm model)
        {
            if (ModelState.IsValid)
            {
                await userAccountManager.UpdateProfile(model);
                SetMessage(MessageType.Success, "Profile updated successfully");
                return RedirectToAction("Index", "Settings");

            }
            //TO DO : Redirect to the Tab ?
            return RedirectToAction("Index");
        }


        //[HttpPost]
        //public ActionResult Password(ChangePasswordVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        userAccountManager.UpdatePassword(model);

        //        var msg = new AlertMessageStore();
        //        msg.AddMessage("success", "Password updated successfully");
        //        TempData["AlertMessages"] = msg;
        //    }
        //    //TO DO : Redirect to the Tab ?
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public ActionResult Settings(DefaultIssueSettings model)
        {

            userAccountManager.SaveDefaultProjectForTeam(model);
            SetMessage(MessageType.Success, "Settings updated successfully");
            return RedirectToAction("Index", "Settings");


        }

    }


}
