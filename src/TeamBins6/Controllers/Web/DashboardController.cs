using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Services;


namespace TeamBins6.Controllers.Web
{

    public class DashboardController : BaseController
    {
        private readonly IUserSessionHelper userSessionHelper;
        private readonly IUserAccountManager userAccountManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        private ITeamManager teamManager;

        public DashboardController(IUserSessionHelper userSessionHelper,IUserAccountManager userAccountManager,IIssueManager issueManager,IProjectManager projectManager,ITeamManager teamManager)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.teamManager = teamManager;

        }

        public async Task<IActionResult> Index(int? id)
        {

            int teamId = userSessionHelper.TeamId;
            var vm = new DashBoardVM {  };
            if (id != null)
            {
                var team = teamManager.GetTeam(id.Value);
                if (team != null && team.IsPublic)
                {
                    teamId = id.Value;
                }
                userSessionHelper.SetTeamId(id.Value);
                await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, id.Value);
            }

            var issues = this.issueManager.GetIssuesGroupedByStatusGroup(teamId,25).SelectMany(f => f.Issues).ToList();
            vm.RecentIssues = issues;

            var myIssues =await issueManager.GetIssuesAssignedToUser(this.userSessionHelper.UserId);
            vm.IssuesAssignedToMe = myIssues;
            vm.Projects = this.projectManager.GetProjects().ToList();

            return View(vm);
        }
    }
}
