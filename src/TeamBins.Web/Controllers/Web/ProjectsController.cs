using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins.Controllers.Web
{
    [LoginCheckFilter]
    public class ProjectsController : Controller
    {
        private readonly IProjectManager projectManager;
        private readonly IUserAuthHelper userAuthHelper;
        public ProjectsController(IProjectManager projectManager, IUserAuthHelper userAuthHelper)
        {
            this.projectManager = projectManager;
            this.userAuthHelper = userAuthHelper;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new TeamProjectListVM();
            var defaultProject = await projectManager.GetDefaultProjectForCurrentTeam();

            vm.Projects = projectManager.GetProjects(this.userAuthHelper.TeamId).Select(s => new ProjectVM
            {
                Id = s.Id,
                Name = s.Name,
                IsDefaultProject = (defaultProject != null && s.Id == defaultProject.Id)

            }).ToList();

            return View(vm);
        }

        public IActionResult Add()
        {
            var addVm = new CreateProjectVM();
            return PartialView("Partial/Add", addVm);
        }
        public IActionResult Edit(int id)
        {
            var addVm = new CreateProjectVM { Id = id };
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
                else
                {
                    var errorMessages = ViewData.ModelState.Values.SelectMany(s => s.Errors.Select(x => x.ErrorMessage));
                    return Json(new { Status = "Error", Errors = errorMessages });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Errors = new List<string> { "Error in saving project" } });
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
            var issueCount = this.projectManager.GetIssueCountForProject(id);
            vm.DependableItemsCount = issueCount;
            return PartialView("Partial/DeleteConfirm", vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            try
            {
                this.projectManager.Delete(model.Id);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Errors = new List<string> { "Error deleting project" } });
            }
        }


    }
}
