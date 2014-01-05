using System;
using System.Web.Mvc;

namespace Planner.Controllers
{
    public class BaseController : Controller
    {
        protected int UserID
        {
            get 
            {
                if (Session["TB_UserID"] != null)
                {
                    return Convert.ToInt32(Session["TB_UserID"]);
                }
                return 7;
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
                return 3;
            }
        }

    }

}
