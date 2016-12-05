using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins.Services;


namespace TeamBins.Controllers.Api
{

    [Route("api/[controller]")]
    public class CommentsApiController : Controller
    {
        private ICommentManager commentManager;
        private IUserAuthHelper userSessionHelper;
        public CommentsApiController(ICommentManager commentManager, IUserAuthHelper userSessionHelper)
        {
            this.commentManager = commentManager;
            this.userSessionHelper = userSessionHelper;
        }

        // GET: api/values
        [HttpGet]
        [Route("~/api/issue/{issueId}/comments")]
        public IEnumerable<CommentVM> Get(int issueId)
        {
            return this.commentManager.GetComments(issueId);
        }

        // POST api/values
        [HttpPost]
        [Route("~/api/comments/{commentId}/delete")]
        public IActionResult DeleteComment(int commentId)
        {
            var comment = this.commentManager.GetComment(commentId);
            if (comment != null && comment.Author.Id == this.userSessionHelper.UserId)
            {
                this.commentManager.Delete(commentId);
                return Json(new { Status = "Success" });

            }
            return Json(new { Status = "Error", Message = "Can not delete comment!" });

        }

        // GET api/values/5


        // POST api/values
        [HttpPost]
        // [Route("")]
        public async Task<IActionResult> Create([FromBody]CommentVM value)
        {
            var commentId = this.commentManager.SaveComment(value);
            var c = this.commentManager.GetComment(commentId);
            this.commentManager.SaveActivity(commentId, value.IssueId);
            await this.commentManager.SendEmailNotificaionForNewComment(c);
            return Json(new { Status = "Success", Data = c });


        }
        
    }
}
