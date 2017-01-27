using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using TeamBins.Common.ViewModels;

namespace TeamBins.Infrastrucutre.Filters
{
    public class LoginCheckFilter : ActionFilterAttribute
    {
        // This is a temp session check filter. Will replace with non session way
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            byte[] val;
            if (context.HttpContext.Session == null || !context.HttpContext.Session.TryGetValue("TB_UserId", out val))
            {
                context.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
            base.OnActionExecuting(context);
        }
    }


    public class SiteAdminLoginCheckFilter : ActionFilterAttribute
    {
        // This is a temp session check filter. Will replace with non session way
        public override void OnActionExecuting(ActionExecutingContext context)
        {


            byte[] emaiVal;
            if (context.HttpContext.Session != null && context.HttpContext.Session.TryGetValue("TB_Email", out emaiVal))
            {
                string result = System.Text.Encoding.UTF8.GetString(emaiVal);
                if (result == "connectshyju@gmail.com")
                {
                    base.OnActionExecuting(context);
                    
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                }
            }
            else
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
           

        }
    }

}