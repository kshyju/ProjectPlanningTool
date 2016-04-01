using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using TeamBins.Common;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class UsersController : Controller
    {
        private ITeamManager teamManager;
        public UsersController(ITeamManager teamManager)
        {
            this.teamManager = teamManager;
        }
        public async Task<IActionResult> Index()
        {

            var teamVm = new TeamVM();


            teamVm = await teamManager.GetTeamInoWithMembers();

            return View(teamVm);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(AddTeamMemberRequestVM model)
        {
            try
            {
                //if (ModelState.IsValid)
                {
                    await teamManager.AddNewTeamMember(model);
                    return Json(new { Status = "Success", Message = "Successfully added user to team" });
                }
                ////else
                //{
                //    var errors = ViewData.ModelState.Values.SelectMany(s => s.Errors.Select(g => g.ErrorMessage));
                //    return Json(new { Status = "Error", Message = "Error adding user to team", Errors= errors });
                //}
            }
            catch (Exception ex)
            {
                //Log
            }

            return Json(new { Status = "Error", Message = "Error adding user to team" });  
        }

    }
}
