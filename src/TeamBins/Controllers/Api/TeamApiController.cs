using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Api
{
    [Route("api/team")]
    public class TeamApiController : Controller
    {
        private ITeamManager teamManager;
        private IUserSessionHelper userSessionHelper;

        public TeamApiController(ITeamManager teamManager,IUserSessionHelper userSessionHelper)
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

        //public ObjectResult GetActivityStream()
        //{
        //    var token = "d";
        //    var Responseresponse = new HttpOkObjectResult(token);

        //    //response.Headers.Add()

        //    Response.Headers.Add("Token","ss");


        //    return Responseresponse;
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
