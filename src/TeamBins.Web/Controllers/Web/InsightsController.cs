using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Infrastrucutre.Services;
using TeamBins.Services;


namespace TeamBins.Web.Controllers.Web
{
    [SiteAdminLoginCheckFilter]
    public class InsightsController : Controller
    {
        private readonly ITeamManager _teamManager;
        private readonly IUserAccountManager _userAccountManager;
        private readonly IIssueManager _issueManager;

        public InsightsController(ITeamManager teamManager,IUserAccountManager userAccountManager,IIssueManager issueManager)
        {
            _teamManager = teamManager;
            _userAccountManager = userAccountManager;
            _issueManager = issueManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new InsightDashboardVm();
            vm.Teams = _teamManager.GetTeams();
            vm.Users = await _userAccountManager.GetAllUsers();
            vm.Issues= _issueManager.GetIssues();
            return View(vm);
        }
    }
}
