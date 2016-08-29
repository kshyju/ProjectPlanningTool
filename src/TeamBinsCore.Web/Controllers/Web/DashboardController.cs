using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Filters;
using TeamBins6.Infrastrucutre.Services;


namespace TeamBins6.Controllers.Web
{
   // [LoginCheckFilter]
    public class DashboardController : BaseController
    {
        private readonly IUserAuthHelper userSessionHelper;
        private readonly IUserAccountManager userAccountManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        private ITeamManager teamManager;

        public DashboardController(IUserAuthHelper userSessionHelper, IUserAccountManager userAccountManager, IIssueManager issueManager, IProjectManager projectManager, ITeamManager teamManager)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.teamManager = teamManager;

        }

        [Route("dashboard/{teamName}")]
        [Route("dashboard")]
        public async Task<IActionResult> Index(string teamName)
        {
            var teamId = userSessionHelper.TeamId;
            var vm = new DashBoardVm { };
            if (!string.IsNullOrEmpty(teamName))
            {
                var team = teamManager.GetTeam(teamName);
                if (team != null && team.IsPublic)
                {
                    teamId = team.Id;
                    vm.TeamKey = teamId.GetHashCode();

                    //If the user who is accessing is already a member of this team, then it is not a public dashboard response
                    if (this.userSessionHelper.TeamId == team.Id && this.userSessionHelper.UserId > 0)
                    {
                        vm.IsCurrentUserTeamMember = true;
                        userSessionHelper.SetTeamId(teamId);
                    }
                }
                else
                {
                    return View("NotFound");
                }
                //
                await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, teamId);
            }
            vm.TeamId = teamId;
            if (userSessionHelper.TeamId > 0)
            {
                vm.IsCurrentUserTeamMember = true;
                var myIssues = await issueManager.GetIssuesAssignedToUser(this.userSessionHelper.UserId);
                vm.IssuesAssignedToMe = myIssues;
            }

            var issues = this.issueManager.GetIssuesGroupedByStatusGroup(teamId, 25).SelectMany(f => f.Issues).ToList();
            vm.RecentIssues = issues;

          
            vm.Projects = this.projectManager.GetProjects(teamId).ToList();

           

            return View(vm);
        }
    }
}
