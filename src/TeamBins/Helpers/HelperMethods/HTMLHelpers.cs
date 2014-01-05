
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechiesWeb.TeamBins.Infrastructure;

namespace TechiesWeb.TeamBins.HelperMethods
{
    public static class HtmlHelpers
    {

        public static IHtmlString AlertMessages(this HtmlHelper helper, string tabId = "")
        {
            var message = "";

            var alertMsgs = helper.ViewContext.Controller.TempData["AlertMessages"] as AlertMessageStore;

            if (alertMsgs != null && alertMsgs.Messages.Count > 0)
            {
                bool isError = false;

                var errors = alertMsgs.Messages.Where(s => s.Key == "error").ToList();
                var successes = alertMsgs.Messages.Where(s => s.Key == "success").ToList();


                isError = alertMsgs.Messages.ToList()[0].Key == "error";
                if (errors.Count > 0)
                {
                    message += "<div class='alert alert-error'><ul>";
                    foreach (var alert in errors)
                    {
                        if (!String.IsNullOrEmpty(alert.Value))
                        {
                            message += String.Format("<li>{0}</li>", alert.Value);
                        }
                    }
                    message += "</ul></div>";
                }
                if (successes.Count > 0)
                {
                    message += "<div class='alert alert-success'><ul>";
                    foreach (var alert in successes)
                    {
                        if (!String.IsNullOrEmpty(alert.Value))
                        {
                            message += String.Format("<li'>{0}</li>", alert.Value);
                        }
                    }
                    message += "</ul></div>";
                }
            }
            return new HtmlString(message);



        }
    }
}