using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Services;
using TeamBins6.Controllers.Web;
using TeamBins6.Infrastrucutre.Services;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TeamBins6.Controllers
{

    public class BaseController : Controller
    {
        public string AppBaseUrl
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));
            }
        }

        protected void SetMessage(MessageType messageType, string message)
        {
            TempData["AlertMessages"] = new Dictionary<string, string> { { messageType.ToString().ToLower(), message } }; 
        }

    }
    public class HomeController : BaseController
    {
        private IUserAuthHelper userSessionHelper;
        private IUserAccountManager userAccountManager;
        private IConfiguration configuration; 

        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager,IConfiguration configuration)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
            this.configuration = configuration;
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
           // HttpContext.Request.HttpContext.Request.
            //ViewContext
           
           // this.userSessionHelper.SetUserIDToSession(new LoggedInSessionInfo { TeamId = 13109, UserId = 12095 });

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
