using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TeamBins.Common;
using TeamBins.Services;
using TeamBins6.Controllers.Web;
using TeamBins6.Infrastrucutre.Services;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace TeamBins6.Controllers
{
    public class HomeController : BaseController
    {
        private IUserAuthHelper userSessionHelper;
        private IUserAccountManager userAccountManager;
        private ILogger<HomeController> logger;

        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager,ILogger<HomeController> logger)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
            this.logger = logger;
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
            this.logger.LogInformation(1,"s000");
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

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
