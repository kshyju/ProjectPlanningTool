using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planner.Controllers;
using Planner.DataAccess;

using TechiesWeb.TeamBins.ViewModels;
using SmartPlan.DataAccess;

namespace SmartPlan.Controllers
{

    public class ProjectsController : BaseController
    {
        private IRepositary repo;
        public ProjectsController()
        {
            repo = new Repositary();
        }

        
        public ActionResult Index(int id)
        {
            var vm = new TeamProjectListVM { TeamID = id };           
            var projectList = repo.GetProjects(id);
            foreach (var project in projectList)
            {
                var projectVM = new ProjectVM { Name = project.Name, ID = project.ID };
                vm.Projects.Add(projectVM);
            }
            return View(vm);
        }
        public ActionResult Details(int id)
        {
            var project = repo.GetProject(id,TeamID);
            if (project != null)
            {
                var projectVm = new ProjectVM { ID = id, Name = project.Name };
                return View(projectVm);
            }
            return View("NotFound");
        }
        public ActionResult Add(int id)
        {
            var vm = new CreateProjectVM { TeamID = id };
            return PartialView("Partial/Add",vm);
        }

        [HttpPost]
        public ActionResult Add(CreateProjectVM model)
        {
            if (ModelState.IsValid)
            {
                var existing = repo.GetProject(model.Name,model.TeamID);
                if((existing!=null) && (existing.ID!=model.ID))
                    return Json(new { Status="Error", Message= "Project name exists"});


                var project = new Project { Name = model.Name, ID=model.ID };
                project.CreatedByID = UserID;
                project.TeamID = model.TeamID;
                var res=repo.SaveProject(project);
                if (res!=null)
                {
                    return Json(new { Status = "Success", Message = "Project created successfully" });
                }
            }
            return Json(new { Status = "Error", Message = "Required fields are missing" });
           
        }

        public ActionResult Edit(int id)
        {
            var project = repo.GetProject(id,TeamID);
            if (project != null)
            {
                var vm = new CreateProjectVM { ID = id, Name = project.Name };
                return PartialView("Partial/Add", vm);
            }
            return View("NotFound");    
        }
        public ActionResult DeleteConfirm(int id)
        {
            var vm = new DeleteProjectConfirmVM();

          //  vm.DependableItemsCount = repo.GetIssues(TeamID).Where(s => s.Project.ID == id).Count();

            return PartialView("Partial/DeleteConfirm",vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            var result = repo.DeleteProject(model.ID, TeamID);
            return Json(new { Status = "Success", Message = "Project deleted successfully" });
        }
    }
}
