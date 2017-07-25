using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre.Services;

namespace TeamBins.Web.Controllers.Api
{
    [Route("api/team")]
    public class TeamApiController : Controller
    {
        private readonly ITeamManager _teamManager;
        private readonly IUserAuthHelper _userSessionHelper;




        public TeamApiController(ITeamManager teamManager, IUserAuthHelper userSessionHelper)
        {
            this._teamManager = teamManager;
            this._userSessionHelper = userSessionHelper;
        }
        // GET: api/values
        [HttpGet]
        [Route("ActivityStream/{teamid}")]
        public IEnumerable<ActivityDto> GetActivityStream(int count, int? teamId)
        {
          //  IDatabase cache = Connection.GetDatabase();
            IEnumerable<ActivityDto> activityList = new List<ActivityDto>();
            var teamIdToUse = this._userSessionHelper.TeamId;
            if (teamId != null)
            {
                var t = _teamManager.GetTeam(teamId.Value);
                if (t != null && t.IsPublic)
                {
                    teamIdToUse = t.Id;
                }
            }
            var cacheKey = Infrastrucutre.CacheKeys.ActivityStream + teamIdToUse;

            //string value = cache.StringGet(cacheKey);
            //if (value == null)
            //{
            //    activityList = this._teamManager.GeActivityItems(teamIdToUse, count);
            //   // cache.StringSet(cacheKey, JsonConvert.SerializeObject(activityList));
            //}
            //else
            //{
            //    activityList = JsonConvert.DeserializeObject<IEnumerable<ActivityDto>>(value);  
            //}
            return activityList;
        }

        [HttpGet]
        [Route("Summary/{teamid}")]
        public async Task<ObjectResult> GetSummary(int teamid, int count)
        {
            return Ok(await this._teamManager.GetDashboardSummary(teamid));
        }
        [HttpGet]
        [Route("IssuesGroupedByPrioroty/{teamid}")]
        public async Task<ObjectResult> GetIssuesGroupedByPrioroty(int teamid, int count)
        {
            return Ok(await this._teamManager.GetIssueCountPerPriority(teamid));
        }

        [HttpGet]
        [Route("IssuesGroupedByProject/{teamid}")]
        public async Task<ObjectResult> GetIssuesGroupedByProject(int teamid, int count)
        {
            return Ok(await this._teamManager.GetIssueCountPerProject(teamid));
        }

    }
}
