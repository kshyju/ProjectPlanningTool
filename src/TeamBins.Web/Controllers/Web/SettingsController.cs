
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins.Controllers;
using TeamBins.Infrastrucutre.Filters;


namespace TeamBins.Controllers
{
    [LoginCheckFilter]
    public class SettingsController : BaseController
    {
        readonly IUserAccountManager _userAccountManager;
        public SettingsController(IUserAccountManager userAccountManager)
        {
            this._userAccountManager = userAccountManager;
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {  
            var vm = new SettingsVm
            {
                Profile = await _userAccountManager.GetUserProfile(),
                NotificationSettings =await _userAccountManager.GetNotificationSettings(),
                IssueSettings = await _userAccountManager.GetIssueSettingsForUser()
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> Notifications(UserEmailNotificationSettingsVM model)
        {
            await _userAccountManager.SaveNotificationSettings(model);
            SetMessage(MessageType.Success,"Settings updated successfully");
            return RedirectToAction("Index", "Settings");
        }
        [HttpPost]
        public async Task<ActionResult> EditProfile(EditProfileVm model)
        {
            if (ModelState.IsValid)
            {
                await _userAccountManager.UpdateProfile(model);
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

            _userAccountManager.SaveDefaultProjectForTeam(model);
            SetMessage(MessageType.Success, "Settings updated successfully");
            return RedirectToAction("Index", "Settings");


        }

    }


}
