using System;
using System.IO;
using System.Web.Mvc;
using TeamBins.Common;
using TeamBins.DataAccess;
using TechiesWeb.TeamBibs.Helpers.Logging;

namespace TechiesWeb.TeamBins.Controllers
{
    public class BaseController : Controller
    {
        protected IRepositary repo;
        protected ILogger log;
        public BaseController()
        {
            log = new Logger();
            repo = new Repositary();
        }
        public BaseController(IRepositary repositary)
        {
            log = new Logger();
            repo = repositary;
        }
        protected string SiteBaseURL
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            }
        }
        protected void UpdateTeam(int teamId)
        {
            Session["TB_TeamID"] = teamId;
        }
        protected void SetCreateAndEditMode(bool mode)
        {
            Session["CreateAndEditMode"] = mode;
        }
        protected void SetUserIDToSession(int userId,int teamId,string nickName)
        {
            Session["TB_UserID"]=userId;
            Session["TB_TeamID"] = teamId;
            Session["TB_NickName"] = nickName;
        }

        protected void SetUserIDToSession(LoggedInSessionInfo sessionInfo)
        {
            Session["TB_UserID"] = sessionInfo.UserId;
            Session["TB_TeamID"] = sessionInfo.TeamId;
            Session["TB_NickName"] = sessionInfo.UserDisplayName;
        }

        protected bool CreateAndEditMode
        {
            get
            {
                if (Session != null && Session["CreateAndEditMode"] != null)
                    return (bool)Session["CreateAndEditMode"];

                return false;
            }
        }
        protected int UserID
        {
            get 
            {
                if (Session != null && Session["TB_UserID"] != null)
                {
                    return Convert.ToInt32(Session["TB_UserID"]);
                }
                return 0;
            }
        }
        protected int TeamID
        {
            get
            {
                if ( Session!=null &&  Session["TB_TeamID"] != null)
                {
                    return Convert.ToInt32(Session["TB_TeamID"]);
                }
                return 0;
            }
        }
        /// <summary>
        /// Returns a view's HTML markup in string format  
        /// </summary>
        /// <param name="viewName">Name of the view</param>
        /// <param name="model">The Model/ViewModel object which is strongly typed to the view</param>
        /// <param name="dictionary">Optional dictionary object</param>
        /// <returns></returns>
        protected string RenderPartialView(string viewName, object model, ViewDataDictionary dictionary = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            this.ViewData.Model = model;
            if (dictionary != null)
            {
                foreach (var item in dictionary.Keys)
                {
                    this.ViewData.Add(item, dictionary[item]);
                }
            }
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public class VerifyLogin : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {

                var session = filterContext.HttpContext.Session;
                if (session["TB_UserID"] != null)
                {
                    return;
                }

                string baseUrl = filterContext.HttpContext.Request.Url.Scheme + "://" + filterContext.HttpContext.Request.Url.Authority +
                       filterContext.HttpContext.Request.ApplicationPath.TrimEnd('/') + "/";

                //Redirect user to login page
                filterContext.Result = new RedirectResult(baseUrl + "account/login");
            }
        }

    }

}
