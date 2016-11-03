using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;

namespace TeamBins.Controllers
{
    public class BaseController : Controller
    {
        public string AppBaseUrl
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));
            }
        }

        protected void SetMessage(MessageType messageType, string message)
        {
            TempData["AlertMessages"] = new Dictionary<string, string> { { messageType.ToString().ToLower(), message } }; 
        }

    }
}