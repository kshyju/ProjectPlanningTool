using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TeamBins.Common.Infrastructure
{
    public static class UrlBuilderHelper
    {
        public static string SiteBaseUrl
        {
            get
            {
                var request = HttpContext.Current.Request;
                var appUrl = HttpRuntime.AppDomainAppVirtualPath;

                if (appUrl != "/") appUrl += "/";

                var baseUrl = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

                return baseUrl;
            }
          
        }
    }
}
