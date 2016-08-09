using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins6.Infrastrucutre.Filters;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    [LoginCheckFilter]
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

                    
                }
                return View(model);
            }
            catch (Exception ex)
            {
                
            }
            return Json(new { Status = "Error" });
        }


        public IActionResult ChangeVisibility(int id)
        {
            var team = teamManager.GetTeam(id);
            if (team != null && team.IsRequestingUserTeamOwner)
            {
                
                return PartialView("Partial/ChangeVisibility",team);
            }
            return PartialView();
            
        }

        [HttpPost]
        public async Task<IActionResult> ChangeVisibility(TeamDto model)
        {
            try
            {
                await this.teamManager.SaveVisibility(model.Id, model.IsPublic);
                return Json(new { Status = "Success", Message = "Team Visiblity updated successfully" });
            }
            catch (Exception)
            {
                return Json(new { Status = "Error", Message = "Error updating team visibility" });
            }

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
            if (team != null)
            {
                var vm = new TeamVM {Id = team.Id, Name = team.Name, IsPublic = team.IsPublic, IsRequestingUserTeamOwner = team.IsRequestingUserTeamOwner};

                return View(vm);
            }
            return View("NotFound");

        }
    }
}

