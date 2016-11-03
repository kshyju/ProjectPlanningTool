using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

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
                    new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Account", action = "Login"}));
            }
            base.OnActionExecuting(context);
        }
    }
}