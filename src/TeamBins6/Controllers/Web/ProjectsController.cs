    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common.ViewModels;
using TeamBins.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class ProjectsController : Controller
    {
        private IProjectManager projectManager;
        public ProjectsController(IProjectManager projectManager)
        {
            this.projectManager = projectManager;
        }
      
        public IActionResult Index()
        {
            var vm = new TeamProjectListVM();
            vm.Projects =  projectManager.GetProjects().Select(s=> new ProjectVM { Id = s.Id, Name = s.Name}).ToList();
            return View(vm);
        }

        public IActionResult Add()
        {
            var addVm = new CreateProjectVM();
            return PartialView("Partial/Add", addVm);
        }
        public IActionResult Edit(int id)
        {
            var addVm = new CreateProjectVM {Id = id};
            var p = projectManager.GetProject(id);
            addVm.Name = p.Name;
           
            return PartialView("Partial/Add", addVm);
        }

        [HttpPost]
        public IActionResult Add(CreateProjectVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    projectManager.Save(model);
                    
                    return Json(new { Status = "Success", Message = "Project created successfully" });

                }
                return Json(new { Status = "Error", Message = "Required fields are missing" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error in saving project" });
            }
        }

        public IActionResult Details(int id)
        {
            var project = projectManager.GetProject(id);
            if (project != null)
            {
                var projectVm = new ProjectDetailsVM { Id = id, Name = project.Name };
                /*
                var projectMembers = project.ProjectMembers.ToList();
                foreach (var item in projectMembers)
                {
                    var member = new MemberVM { Name = item.Member.FirstName, JobTitle = item.Member.JobTitle };
                    projectVm.Members.Add(member);
                }*/
                return View(projectVm);
            }
            return View("NotFound");
        }

        public ActionResult DeleteConfirm(int id)
        {
            var vm = new DeleteProjectConfirmVM();
          //  vm.DependableItemsCount = repo.GetIssues().Where(s => s.Project.ID == id).Count();
            return PartialView("Partial/DeleteConfirm", vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            try
            {
              //  var result = repo.DeleteProject(model.Id);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {
               
                return Json(new { Status = "Error", Message = "Error deleting project" });
            }
        }
    }
}
