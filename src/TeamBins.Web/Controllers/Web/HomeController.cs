using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;
using TeamBins.Controllers;
using TeamBins.Infrastrucutre;
using TeamBins.Services;

namespace TeamBins.Web.Controllers.Web
{
    public class HomeController : BaseController
    {
        private IHostingEnvironment _env;
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly IUserAccountManager _userAccountManager;
        
        public HomeController(IUserAuthHelper userSessionHelper,IUserAccountManager userAccountManager,
            IOptions<AppSettings> settings, IHostingEnvironment env) : base(settings)
        {
            this._userSessionHelper = userSessionHelper;
            this._userAccountManager = userAccountManager;
            tc = new TelemetryClient() { InstrumentationKey = settings.Value.ApplicationInsights.InstrumentationKey };
            this._env = env;
        }
      
        [HttpPost]
        public async Task<IActionResult> SwitchTeam(int teamId)
        { 
            _userSessionHelper.SetTeamId(teamId);
            await _userAccountManager.SetDefaultTeam(_userSessionHelper.UserId, teamId);
            tc.TrackEvent("Switching Team");
            return Json("Changed");
        }

        public ActionResult Track(string id="devstory")
        {
            tc.TrackPageView(id);
            var webRoot = _env.WebRootPath+ "/css/images/bug-icon.png";
            return PhysicalFile(webRoot, "image/jpeg");
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
