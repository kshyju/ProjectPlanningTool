using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackExchange.Exceptional;
using TeamBins.DataAccess;
using TeamBins.Services;

namespace TechiesWeb.TeamBins.Controllers
{
    //[VerifyLogin]
    public class DevController : BaseController
    {
        public ActionResult Exceptions()
        {
            var context = System.Web.HttpContext.Current;
            var page = new HandlerFactory().GetHandler(context, Request.RequestType, Request.Url.ToString(), Request.PathInfo);
            page.ProcessRequest(context);

            return null;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]      
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult UpdateAvatars(string secret)
        {
            int userCount=0;
            try
            {
                if (secret == "TeamBins")
                {
                    IRepositary repo = new Repositary();
                    var users = repo.GetUsers().Where(s => String.IsNullOrEmpty(s.Avatar)).ToList();
                    foreach (var user in users)
                    {
                        user.Avatar = UserService.GetGravatarHash(user.EmailAddress);
                        var result = repo.SaveUser(user);
                        if (result.Status)
                            userCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message + ex.InnerException != null ? ex.InnerException.ToString() : "");
            }
            return Content(userCount.ToString() + " user's avatar updated");
        }
	}
}