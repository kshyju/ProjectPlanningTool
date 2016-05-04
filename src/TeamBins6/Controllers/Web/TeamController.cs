using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class TeamController : Controller
    {
        private ITeamManager teamManager;
        public TeamController(ITeamManager teamManager)
        {
            this.teamManager = teamManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var teams = this.teamManager.GetTeams();
            return View(new TeamListVM {  Teams = teams});
        }
        public ActionResult Add()
        {
            return PartialView("Edit", new TeamVM());
        }
        public ActionResult Edit(int id)
        {
            var team = teamManager.GetTeam(id);
            if (team != null)
            {
                var vm = new TeamVM { Name = team.Name, Id = team.Id };
                return PartialView(vm);
            }
            return View("NotFound");
        }

        [HttpPost]
        public ActionResult Create(TeamVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Name = model.Name.Replace(" ", "");
                    var teamId = teamManager.SaveTeam(model);
                    return Json(new { Status = "Success" });

                    //Team team = new Team { Name = model.Name, ID = model.Id };
                    //bool isNew = (model.Id == 0);
                    //if (!isNew)
                    //{
                    //    team = repo.GetTeam(model.Id);
                    //    team.Name = model.Name;
                    //}
                    //else
                    //{
                    //    team.CreatedByID = UserID;
                    //}
                    //var result = repo.SaveTeam(team);
                    //if (result != null)
                    //{
                    //    if (isNew)
                    //    {
                    //        var teamMember = new TeamMember { MemberID = UserID, TeamID = team.ID, CreatedByID = UserID };
                    //        repo.SaveTeamMember(teamMember);
                    //    }
                    //    return Json(new { Status = "Success" });
                    //}
                }
                return View(model);
            }
            catch (Exception ex)
            {
                
            }
            return Json(new { Status = "Error" });
        }



        public ActionResult DeleteConfirm(int id)
        {
            var vm = new DeleteProjectConfirmVM();
            // to do check : based on permission, check member count, issue count etc
           
            return PartialView("Partial/DeleteConfirm", vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            try
            {
                this.teamManager.Delete(model.Id);
                //  var result = repo.DeleteProject(model.Id);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { Status = "Error", Message = "Error deleting project" });
            }
        }

        public IActionResult View(int id)
        {
            var team = this.teamManager.GetTeam(id);
            if (team != null && team.IsRequestingUserTeamOwner)
            {

                return View(team);
            }
            return View("NotFound");

        }
    }
}
