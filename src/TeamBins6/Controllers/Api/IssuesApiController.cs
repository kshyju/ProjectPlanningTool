using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers
{
    [Route("api/issues")]
    public class IssuesApiController : Controller
    {

        ICommentManager commentManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        //private IssueService issueService;
        IUserSessionHelper userSessionHelper;


        public IssuesApiController(ICommentManager commentManager, IUserSessionHelper userSessionHelper, IProjectManager projectManager, IIssueManager issueManager) //: base(repositary)
        {
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.userSessionHelper = userSessionHelper;
            this.commentManager = commentManager;
        }


        [HttpPost]
        [Route("~/api/issues/{issueId}/star/{mode}")]
        public async Task<IActionResult> StarIssue(bool mode, int issueId)
        {
            await issueManager.StarIssue(issueId, 0, !mode);
            if (!mode)
            {
              
                return Json(new {Status = "Success", Class = "glyphicon-star", Starred = true});
            }
            else
            {
                return Json(new { Status = "Success", Class = "glyphicon-star-empty", Starred = false });
            }
        }

        [HttpGet]
        [Route("~/api/issues/{issueId}/noissuemembers")]
        public async Task<IActionResult> GetIssueMembers(string term,int issueId)
        {
            var list= await issueManager.GetNonIssueMembers(issueId);
            return Json(list);
        }

        [HttpPost]
        [Route("~/api/issues/{issueId}/assignteammember/{userId}")]
        public async Task<IActionResult> GetIssueMembers(int issueId,int userId)
        {
            await this.issueManager.SaveIssueAssignee(issueId,userId);
            return Json( new { Status="Success"});
        }
        [HttpPost]
        [Route("~/api/issues/{issueId}/removeissuemember/{userId}")]
        public async Task<IActionResult> RemoveIssueMembers(int issueId, int userId)
        {
            await this.issueManager.RemoveIssueMember(issueId, userId);
            return Json(new { Status = "Success" });
        }
        // GET api/values/5
        [HttpGet("{teamId}/{count}")]
        public ObjectResult Get(int teamId,int count)
        {

            var issues= this.issueManager.GetIssuesGroupedByStatusGroup(teamId,count);

            return Ok(issues);
        }

        [HttpPost]
        [Route("~/api/issues/{id}/SaveDueDate")]
        public async Task<ObjectResult> SaveDueDate(int id, DateTime? issueDueDate)
        {
            try
            {
                var issue = this.issueManager.GetIssue(id);
                if (issue != null)
                {
                    await this.issueManager.SaveDueDate(id, issueDueDate);
                }
                return Ok(new { Status = "Success" });
            }
            catch (Exception)
            {

                return Ok(new { Status = "Error", Message = "Can not delete comment!" });
            }
        }
        [HttpPost]
        [Route("~/api/issue/{id}/delete")]
        public ObjectResult Delete(int id, string token = "")
        {
            try
            {
                var issue = this.issueManager.GetIssue(id);
                if (issue != null && issue.Author.Id == this.userSessionHelper.UserId)
                {
                    this.issueManager.Delete(id);
                }
                ///return new HttpOkObjectResult();
                return  Ok(new { Status = "Success" });
            }
            catch (Exception)
            {

                return Ok(new { Status = "Error", Message = "Can not delete comment!" });
            }
        }
    }
}
