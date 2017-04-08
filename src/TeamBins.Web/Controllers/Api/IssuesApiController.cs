using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins.Web.Controllers.Api
{
    [Route("api/issues")]
    public class IssuesApiController : Controller
    {
        private readonly IIssueManager _issueManager;
        readonly IUserAuthHelper _userSessionHelper;


        public IssuesApiController(ICommentManager commentManager, IUserAuthHelper userSessionHelper, IProjectManager projectManager, IIssueManager issueManager) //: base(repositary)
        {
            this._issueManager = issueManager;
            this._userSessionHelper = userSessionHelper;
        }


        [HttpPost]
        [Route("~/api/issues/{issueId}/star/{mode}")]
        public async Task<IActionResult> StarIssue(bool mode, int issueId)
        {
            await _issueManager.StarIssue(issueId, 0, !mode);
            if (!mode)
            {
                return Json(new { Status = "Success", Class = "glyphicon-star", Starred = true });
            }
            else
            {
                return Json(new { Status = "Success", Class = "glyphicon-star-empty", Starred = false });
            }
        }

        [HttpGet]
        [Route("~/api/issues/{issueId}/noissuemembers")]
        public async Task<IActionResult> GetIssueMembers(string term, int issueId)
        {
            var list = await _issueManager.GetNonIssueMembers(issueId);
            return Json(list);
        }

        [HttpPost]
        [Route("~/api/issues/{issueId}/assignteammember/{userId}")]
        public async Task<IActionResult> GetIssueMembers(int issueId, int userId)
        {
            await this._issueManager.SaveIssueAssignee(issueId, userId);
            return Json(new { Status = "Success" });
        }
        [HttpPost]
        [Route("~/api/issues/{issueId}/removeissuemember/{userId}")]
        public async Task<IActionResult> RemoveIssueMembers(int issueId, int userId)
        {
            await this._issueManager.RemoveIssueMember(issueId, userId);
            return Json(new { Status = "Success" });
        }
        // GET api/values/5
        [HttpGet("{teamId}/{count}")]
        public ObjectResult Get(int teamId, int count)
        {

            var issues = this._issueManager.GetIssuesGroupedByStatusGroup(teamId, count);

            return Ok(issues);
        }

        [HttpPost]
        [Route("~/api/issues/{id}/SaveDueDate")]
        public async Task<ObjectResult> SaveDueDate(int id, DateTime? issueDueDate)
        {
            try
            {
                var issue = this._issueManager.GetIssue(id);
                if (issue != null)
                {
                    await this._issueManager.SaveDueDate(id, issueDueDate);
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
        public async Task<ObjectResult> Delete(int id, string token = "")
        {
            try
            {
                var issue = await this._issueManager.GetIssue(id);
                if (issue != null && issue.Author.Id == this._userSessionHelper.UserId)
                {
                    this._issueManager.Delete(id);
                }
                return Ok(new { Status = "Success" });
            }
            catch (Exception)
            {

                return Ok(new { Status = "Error", Message = "Can not delete comment!" });
            }
        }
    }
}
