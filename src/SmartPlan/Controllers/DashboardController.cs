using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planner.Controllers;
using Planner.DataAccess;
using SmartPlan.ViewModels;


namespace SmartPlan.Controllers
{
    public class DashboardController : BaseController
    {
        //
        // GET: /Dashboard/
        IRepositary repo;

        public DashboardController()
        {
            repo=new Repositary();
        }

        public ActionResult Index()
        {
            var vm = new DashBoardVM();
            
            var teams = repo.GetTeams(UserID);
            foreach (var item in teams)
            {
                vm.Teams.Add(new TeamVM { ID = item.ID, Name = item.Name });
            }



            return View(vm);
        }

    }
}
