using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins.Controllers.Web
{
    [LoginCheckFilter]
    public class TeamController : Controller
    {
        private IUserAuthHelper _userAuthHelper;
        private ITeamManager _teamManager;
        public TeamController(ITeamManager teamManager,IUserAuthHelper userAuthHelper)
        {
            this._teamManager = teamManager;
            this._userAuthHelper = userAuthHelper;
        }

        public IActionResult Index()
        {
            var teams = this._teamManager.GetTeams();
            return View(new TeamListVM {  Teams = teams});
        }
        public ActionResult Add()
        {
            return PartialView("Edit", new TeamVM());
        }
        public ActionResult Edit(int id)
        {
            var team = _teamManager.GetTeam(id);
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
                    var teamId = _teamManager.SaveTeam(model);
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
            var team = _teamManager.GetTeam(id);
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
                await this._teamManager.SaveVisibility(model.Id, model.IsPublic);
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
                this._teamManager.Delete(model.Id);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { Status = "Error", Message = "Error deleting project" });
            }
        }

        public IActionResult View(int id)
        {
            var team = this._teamManager.GetTeam(id);
            if (team != null)
            {
                var vm = new TeamVM {Id = team.Id, Name = team.Name, IsPublic = team.IsPublic, IsRequestingUserTeamOwner = team.IsRequestingUserTeamOwner};

                return View(vm);
            }
            return View("NotFound");

        }


    }
}

