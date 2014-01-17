using System;
using System.Web.Mvc;
using TechiesWeb.TeamBibs.Helpers.Logging;

namespace Planner.Controllers
{
    public class BaseController : Controller
    {
        protected ILogger log;
        public BaseController()
        {
            log = new Logger();
        }
        protected void UpdateTeam(int teamId)
        {
            Session["TB_TeamID"] = teamId;
        }
        protected void SetUserIDToSession(int userId,int teamId,string nickName)
        {
            Session["TB_UserID"]=userId;
            Session["TB_TeamID"] = teamId;
            Session["TB_NickName"] = nickName;
        }       
        protected int UserID
        {
            get 
            {
                if (Session["TB_UserID"] != null)
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
                if (Session["TB_TeamID"] != null)
                {
                    return Convert.ToInt32(Session["TB_TeamID"]);
                }
                return 0;
            }
        }

    }

}
