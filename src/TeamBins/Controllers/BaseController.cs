using System;
using System.Web.Mvc;

namespace Planner.Controllers
{
    public class BaseController : Controller
    {        
        protected void SetUserIDToSession(int userId,int teamId)
        {
            Session["TB_UserID"]=userId;
            Session["TB_TeamID"] = teamId;
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
