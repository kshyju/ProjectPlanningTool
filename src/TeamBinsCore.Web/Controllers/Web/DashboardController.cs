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
                if (team != null)
                {
                    //If the current user who is accessing is already a member of this team,
                    if (teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId,team.Id))
                    {
                        vm.IsCurrentUserTeamMember = true;
                        //userSessionHelper.SetTeamId(team.Id);
                        //await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, teamId);
                    }
                    else
                    {
                        // He is either accessing a public dashboard or TRYING to peep into a private dashboard

                        if (team.IsPublic)
                        {
                            teamId = team.Id;
                            vm.TeamKey = teamId.GetHashCode();
                        }
                        else
                        {
                            return View("NotFound");
                        }
                    }
                }
                else
                {
                    return View("NotFound");
                }
            }
            vm.TeamId = teamId;
            if (userSessionHelper.TeamId > 0 )
            {
                if (teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId, teamId))
                {
                    vm.IsCurrentUserTeamMember = true;
                    var myIssues = await issueManager.GetIssuesAssignedToUser(this.userSessionHelper.UserId);
                    vm.IssuesAssignedToMe = myIssues;
                }
            }

            var issues = this.issueManager.GetIssuesGroupedByStatusGroup(teamId, 25).SelectMany(f => f.Issues).ToList();
            vm.RecentIssues = issues;


            vm.Projects = this.projectManager.GetProjects(teamId).ToList();



            return View(vm);
        }
    }
}
