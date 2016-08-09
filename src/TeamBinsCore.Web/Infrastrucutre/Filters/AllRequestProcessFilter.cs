using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using TeamBins6.Controllers;
using TeamBins6.Controllers.Web;

namespace TeamBins6.Infrastrucutre.Filters
{

    public class ReqProcessFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var c = context.Controller as BaseController;
            if (c != null)
            {
                c.ViewBag.AppBaseUrl = c.AppBaseUrl;
            }

            base.OnActionExecuting(context);
        }
    }

    public class LoginCheckFilter : ActionFilterAttribute
    {
        // This is a temp session check filter. Will replace with non session way
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            byte[] val;
            if (context.HttpContext.Session == null || !context.HttpContext.Session.TryGetValue("TB_UserId", out val))
            {
                context.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Account", action = "Login"}));
            }
            base.OnActionExecuting(context);
        }
    }
}
