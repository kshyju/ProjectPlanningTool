using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TeamBins.Common.ViewModels;
using TeamBins.Services;

namespace TeamBins.Controllers
{
    public class IssueApiController : ApiController
    {
        IssueManager issueManager;
        ICommentManager commentManager;
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
            var statusIds = new List<int> { 1, 2, 3, 4 };
            var issueVMs = issueManager.GetIssues(statusIds, 50).ToList();
            return Request.CreateResponse(HttpStatusCode.OK,issueVMs);
        }

        [Route("api/issues/{id}/comments")]
        [HttpGet]
        public HttpResponseMessage GetComments(int id)
        {
            var comments = commentManager.GetComments(id);
            return Request.CreateResponse(HttpStatusCode.OK, comments);
        }
    }
}