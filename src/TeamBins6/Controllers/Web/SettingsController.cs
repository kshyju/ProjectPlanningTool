
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre;


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
        public async Task<ActionResult> Index()
        {
            var vm = new SettingsVm
            {
                Profile = await userAccountManager.GetUserProfile(),
                //NotificationSettings = userAccountManager.GetNotificationSettings(),
                IssueSettings = await userAccountManager.GetIssueSettingsForUser()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> EditProfile(EditProfileVm model)
        {
            if (ModelState.IsValid)
            {
                await userAccountManager.UpdateProfile(model);
                var msg = new AlertMessageStore();
                msg.AddMessage("success", "Profile updated successfully");
                TempData["AlertMessages"] = msg;
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
            if (ModelState.IsValid)
            {
              //  userAccountManager.SaveDefaultProjectForTeam(model.SelectedProject);

                //var msg = new AlertMessageStore();
                //msg.AddMessage("success", "Settings updated successfully");
                //TempData["AlertMessages"] = msg;
                return RedirectToAction("settings");

            }
            //
            //  model.Projects = GetProjectListItem();
            return RedirectToAction("Index");
        }

    }


}
