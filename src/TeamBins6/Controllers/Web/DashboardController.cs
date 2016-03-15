using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class DashboardController : Controller
    {
        private IUserSessionHelper userSessionHelper;
        private IUserAccountManager userAccountManager;

        public DashboardController(IUserSessionHelper userSessionHelper,IUserAccountManager userAccountManager)
        {
            this.userSessionHelper = userSessionHelper;
            this.userAccountManager = userAccountManager;

        }

        // GET: /<controller>/
        [Route("~/Dashboard/{id}")]
        public async Task<IActionResult> Index(int? id)
        {
            var vm = new DashBoardVM {  };
            if (id != null)
            {
                userSessionHelper.SetTeamId(id.Value);
                await userAccountManager.SetDefaultTeam(userSessionHelper.UserId, id.Value);
            }



            return View(vm);
        }
    }
}
