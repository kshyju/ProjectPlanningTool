using System.Threading.Tasks;
using TeamBins.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TeamBins.Common.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamBins.Infrastrucutre;

namespace TeamBins.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserAuthHelper userSessionHelper;
        private readonly IUserAccountManager userAccountManager;
        private readonly ILogger<HomeController> logger;
       

        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
        }
      
        [HttpPost]
        public async Task<IActionResult> SwitchTeam(int teamId)
        { 
            userSessionHelper.SetTeamId(teamId);
            await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, teamId);

            return Json("Changed");
        }

        public IActionResult Index()
        {
            

            if (this.userSessionHelper.UserId > 0)
            {
                return RedirectToAction("Index", "Issues");
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
