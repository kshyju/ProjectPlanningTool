using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using TeamBins6.Controllers;

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
}
