using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using TeamBins.Services;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.ViewModels;
using Microsoft.Extensions.Options;
using TeamBins.Infrastrucutre;

namespace TeamBins.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly IUserAccountManager _userAccountManager;
        
        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager, IOptions<AppSettings> settings) : base(settings)
        {
            this._userSessionHelper = userSessionHelper;
            this._userAccountManager = userAccountManager;
            tc = new TelemetryClient() { InstrumentationKey = settings.Value.ApplicationInsights.InstrumentationKey };
        }
      
        [HttpPost]
        public async Task<IActionResult> SwitchTeam(int teamId)
        { 
            _userSessionHelper.SetTeamId(teamId);
            await _userAccountManager.SetDefaultTeam(_userSessionHelper.UserId, teamId);
            tc.TrackEvent("Switching Team");
            return Json("Changed");
        }

        public IActionResult Index()
        {
            if (this._userSessionHelper.UserId > 0)
            {
                return RedirectToAction("Index", "Issues");
            }
            tc.TrackEvent("Home page view(Unauthenticated)");
            return View();
        }

        public IActionResult About()
        {
            tc.TrackEvent("About page view");
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
