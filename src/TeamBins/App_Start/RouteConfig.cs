using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeamBins
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

          
            
            routes.MapRoute(name: "switchteam",
                url: "t/{teamid}/{teamname}",
                defaults: new { controller = "Dashboard", action = "index" }
                );

            routes.MapRoute(name: "teamissuespublic",
                 url: "tp/{teamid}/{teamname}",
                defaults: new { controller = "Issues", action = "index", teamname=UrlParameter.Optional }

            );
            /*
            routes.MapRoute(name: "issue-comment-direct-link",
                 url: "issuecomment/{commentid}/{issuetitle}",
                defaults: new { controller = "Issues", action = "details", id = 0, issuetitle = UrlParameter.Optional }
            );
            */
           /* */
            routes.MapMvcAttributeRoutes();
           
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
            
        }
    }
}