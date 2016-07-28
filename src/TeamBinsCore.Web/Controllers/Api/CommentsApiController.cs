using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Api
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
                return  Json(new { Status = "Success" });

            }
            return Json(new { Status = "Error", Message = "Can not delete comment!" });

        }

        // GET api/values/5


        // POST api/values
        [HttpPost]
        // [Route("")]
        public IActionResult Create([FromBody]CommentVM value)
        {
            var commentId = this.commentManager.SaveComment(value);
            var c = this.commentManager.GetComment(commentId);
            this.commentManager.SaveActivity(commentId, value.IssueId);
            return Json(new { Status = "Success", Data = c });


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
