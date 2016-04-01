using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{

    public class DashboardController : BaseController
    {
        private readonly IUserSessionHelper userSessionHelper;
        private readonly IUserAccountManager userAccountManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;

        public DashboardController(IUserSessionHelper userSessionHelper,IUserAccountManager userAccountManager,IIssueManager issueManager,IProjectManager projectManager)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
            this.issueManager = issueManager;
            this.projectManager = projectManager;

        }

        // GET: /<controller>/
        //[Route("~/Dashboard/{id}")]
        public async Task<IActionResult> Index(int? id)
        {
           
            var vm = new DashBoardVM {  };
            if (id != null)
            {
                userSessionHelper.SetTeamId(id.Value);
                await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, id.Value);
            }

            var issues = this.issueManager.GetIssuesGroupedByStatusGroup(25).SelectMany(f => f.Issues).ToList();
            vm.RecentIssues = issues;

            vm.Projects = this.projectManager.GetProjects().ToList();

            return View(vm);
        }
    }
}
