using System;
using System.Web;
using Glimpse.AspNet.Tab;
using TeamBins.Common;

namespace TeamBins.Services
{
    public class UserSessionHelper : IUserSessionHelper
    {
        public int UserId
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["TB_UserID"] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session["TB_UserID"]);
                }
                return 0;
            }
        }
        public int TeamId
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["TB_TeamID"] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session["TB_TeamID"]);
                }
                return 0;
            }
        }


        protected void SetUserIDToSession(LoggedInSessionInfo sessionInfo)
        {
            HttpContext.Current.Session["TB_UserID"] = sessionInfo.UserId;
            HttpContext.Current.Session["TB_TeamID"] = sessionInfo.TeamId;
            HttpContext.Current.Session["TB_NickName"] = sessionInfo.UserDisplayName;
        }

    }
}