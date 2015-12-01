using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using StackExchange.Exceptional;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;

namespace TeamBins.Controllers
{
    public class IssueApiController : ApiController
    {
        readonly IssueManager issueManager;
        readonly ICommentManager commentManager;
        public IssueApiController(IssueManager issueManager,ICommentManager commentManager)
        {
            this.issueManager = issueManager;
            this.commentManager = commentManager;
        }
        // GET api/<controller>

        [Route("api/issues/{count}")]
        [HttpGet]
        public HttpResponseMessage Get(int count=50)
        {
            IEnumerable<IssuesPerStatusGroup> groupedIssues = new List<IssuesPerStatusGroup>();
            try
            {
                groupedIssues = issueManager.GetIssuesGroupedByStatusGroup(count);
            }
            catch (Exception ex)
            {
                ErrorStore.LogException(ex, System.Web.HttpContext.Current);
            }
            return Request.CreateResponse(HttpStatusCode.OK, groupedIssues);

        }

        [Route("api/issues/{id}/comments")]
        [HttpGet]
        public HttpResponseMessage GetComments(int id)
        {
            IEnumerable<CommentVM> comments = new List<CommentVM>();
            try
            {
                comments = commentManager.GetComments(id);
            }
            catch (Exception ex)
            {
                ErrorStore.LogException(ex, System.Web.HttpContext.Current);
            }
            return Request.CreateResponse(HttpStatusCode.OK, comments);
        }

    }
}