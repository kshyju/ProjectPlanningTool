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
        private ITeamManager teamManager;
        private IUserAuthHelper userSessionHelper;

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
        [Route("Summary")]
        public async Task<ObjectResult> GetSummary(int count)
        {
            return Ok(await this.teamManager.GetDashboardSummary());
        }
        
    }
}
