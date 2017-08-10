using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre;
using TeamBins.Services;
using TeamBins.Infrastrucutre.Services;


namespace TeamBins.Controllers.Web
{
    // [LoginCheckFilter]
    public class DashboardController : BaseController
    {
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly IProjectManager _projectManager;
        private readonly IIssueManager _issueManager;
        private readonly ITeamManager _teamManager;

        public DashboardController(IUserAuthHelper userSessionHelper, IIssueManager issueManager,
            IProjectManager projectManager, ITeamManager teamManager, IOptions<AppSettings> settings) : base(settings)
        {
            this._userSessionHelper = userSessionHelper;
            this._issueManager = issueManager;
            this._projectManager = projectManager;
            this._teamManager = teamManager;

        }

        [Route("dashboard/{teamName}")]
        [Route("dashboard")]
        public async Task<IActionResult> Index(string teamName)
        {
            tc.TrackEvent("Dashboard view");
            var vm = new DashBoardVm { };
            try
            {
                var teamId = _userSessionHelper.TeamId;

                if (teamId == 0 && string.IsNullOrEmpty(teamName))
                {
                    return RedirectToAction("Login", "account");
                }

                if (!string.IsNullOrEmpty(teamName))
                {
                    var team = _teamManager.GetTeam(teamName);
                    if (team != null)
                    {
                        //If the current user who is accessing is already a member of this team,
                        if (_teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, team.Id))
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

                                tc.TrackEvent("Public dashboard view-" + team.Name.ToLower());

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
                if (_userSessionHelper.TeamId > 0)
                {
                    if (_teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, teamId))
                    {
                        vm.IsCurrentUserTeamMember = true;
                        var myIssues = await _issueManager.GetIssuesAssignedToUser(this._userSessionHelper.UserId);
                        vm.OverDueIssuesAssignedToMe = myIssues
                            .Where(d => d.DueDate != null && d.DueDate.Value < DateTime.Now)
                            .Where(c => c.Status.Code != "Completed" || c.Status.Code != "Closed").OrderBy(f => f.DueDate);
                        vm.IssuesAssignedToMe = myIssues;
                    }
                }

                var issues =
                    this._issueManager.GetIssuesGroupedByStatusGroup(teamId, 25)
                        .SelectMany(f => f.Issues)
                        .OrderByDescending(s => s.CreatedDate)
                        .ToList();
                vm.RecentIssues = issues;

                vm.Projects = this._projectManager.GetProjects(teamId).ToList();
            }
            catch (Exception ex)
            {
                tc.TrackException(ex);
            }
            return View(vm);
        }


    }
}
