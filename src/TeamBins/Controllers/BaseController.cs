using System;
using System.IO;
using System.Web.Mvc;
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
       

    }

}
