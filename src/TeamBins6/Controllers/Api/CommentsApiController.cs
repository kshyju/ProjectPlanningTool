using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common;
using TeamBins.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Api
{

    [Route("api/[controller]")]
    public class CommentsApiController : Controller
    {
        private ICommentManager commentManager;
        public CommentsApiController(ICommentManager commentManager)
        {
            this.commentManager = commentManager;
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
        [Route("{commentId}/delete")]
        public ObjectResult DeleteComment(int commentId)
        {
            this.commentManager.Delete(commentId);
            return new HttpOkObjectResult(new { Status = "Success"});

        }

        // GET api/values/5


        // POST api/values
        [HttpPost]
        public CommentVM Post([FromBody]CommentVM value)
        {
            var commentId = this.commentManager.SaveComment(value);
            return this.commentManager.GetComment(commentId);

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
