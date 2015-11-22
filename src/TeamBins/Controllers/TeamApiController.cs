using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins.Services;

namespace TeamBins.Controllers
{
    public class TeamApiController : ApiController
    {
        ITeamManager teamManager;
        IActivityRepository activityRepository;
        public TeamApiController(ITeamManager teamManager)
        {
            this.teamManager = teamManager;
           
        }

        // GET api/<controller>
       // [Route("activitystream")]
       [Route("api/team/activitystream")]
        [HttpGet]
        public IEnumerable<ActivityDto> GetActivtyStream(int count)
       {
            return teamManager.GeActivityItems(count);
           
        }


    }
}