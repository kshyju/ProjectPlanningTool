using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Infrastrucutre;

namespace TeamBins.Controllers
{
    public class BaseController : Controller
    {
        protected TelemetryClient tc;
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

        public BaseController(IOptions<AppSettings> settings)
        {
            tc = new TelemetryClient() { InstrumentationKey = settings.Value.ApplicationInsights.InstrumentationKey };
        }
    }
}