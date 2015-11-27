using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using StackExchange.Exceptional;
using TeamBins.Services;

namespace TeamBins.Controllers
{
    public class CommentApiController : ApiController
    {
        IssueManager issueManager;
        ICommentManager commentManager;
        IUserSessionHelper userSessionHelper;
        public CommentApiController(IssueManager issueManager, ICommentManager commentManager,IUserSessionHelper userSessionHelper)
        {
            this.issueManager = issueManager;
            this.commentManager = commentManager;
            this.userSessionHelper = userSessionHelper;
        }

        [Route("api/comment/{id}/delete")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteComment(int id)
        {
            try
            {
                var comment = commentManager.GetComment(id);
                if (comment != null&& userSessionHelper.UserId==comment.Author.Id)
                {
                    await commentManager.Delete(id);
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Status = "Success"
                    });
                }

            }
            catch (Exception ex)
            {
                ErrorStore.LogException(ex, System.Web.HttpContext.Current);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = "Error"
            });
        }


    }
}