using System.Threading.Tasks;
using TeamBins.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TeamBins.Common.ViewModels;
using Microsoft.Extensions.Options;
using TeamBins.Infrastrucutre;

namespace TeamBins.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly IUserAccountManager _userAccountManager;


        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager)
        {
            this._userSessionHelper = userSessionHelper;
            this._userAccountManager = userAccountManager;
        }
      
        [HttpPost]
        public async Task<IActionResult> SwitchTeam(int teamId)
        { 
            _userSessionHelper.SetTeamId(teamId);
            await _userAccountManager.SetDefaultTeam(_userSessionHelper.UserId, teamId);

            return Json("Changed");
        }

        public IActionResult Index()
        {
            

            if (this._userSessionHelper.UserId > 0)
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
