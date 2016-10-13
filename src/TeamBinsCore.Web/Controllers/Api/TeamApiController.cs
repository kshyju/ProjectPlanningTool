using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins6.Infrastrucutre.Services;

namespace TeamBins6.Controllers.Api
{
    [Route("api/team")]
    public class TeamApiController : Controller
    {
        private readonly ITeamManager teamManager;
        private readonly IUserAuthHelper userSessionHelper;

        public TeamApiController(ITeamManager teamManager,IUserAuthHelper userSessionHelper)
        {
            this.teamManager = teamManager;
            this.userSessionHelper = userSessionHelper;
        }
       // GET: api/values
       [HttpGet]
       [Route("ActivityStream/{teamid}")]
        public IEnumerable<ActivityDto> GetActivityStream(int count,int? teamId)
       {
           var teamIdToUse = this.userSessionHelper.TeamId;
           if (teamId != null)
           {
               var t = teamManager.GetTeam(teamId.Value);
               if (t != null && t.IsPublic)
               {
                   teamIdToUse = t.Id;
               }
           }
            return this.teamManager.GeActivityItems(teamIdToUse,count);
        }

        [HttpGet]    
        [Route("Summary/{teamid}")]
        public async Task<ObjectResult> GetSummary(int teamid,int count)
        {
            return Ok(await this.teamManager.GetDashboardSummary(teamid));
        }
        [HttpGet]
        [Route("IssuesGroupedByPrioroty/{teamid}")]
        public async Task<ObjectResult> GetIssuesGroupedByPrioroty(int teamid,int count)
        {
            return Ok(await this.teamManager.GetIssueCountPerPriority(teamid));
        }

        [HttpGet]
        [Route("IssuesGroupedByProject/{teamid}")]
        public async Task<ObjectResult> GetIssuesGroupedByProject(int teamid, int count)
        {
            return Ok(await this.teamManager.GetIssueCountPerProject(teamid));
        }

    }
}
