using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TeamBins.Common.ViewModels;
using TeamBins.Controllers;
using TeamBins.Infrastrucutre;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Services;


namespace TeamBins.Web.Controllers.Web
{
    [LoginCheckFilter]
    public class ProjectsController : BaseController
    {
        private readonly IProjectManager _projectManager;
        private readonly IUserAuthHelper _userAuthHelper;
        public ProjectsController(IProjectManager projectManager, IUserAuthHelper userAuthHelper, IOptions<AppSettings> settings) : base(settings)
        {
            this._projectManager = projectManager;
            this._userAuthHelper = userAuthHelper;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new TeamProjectListVM();
            try
            {
                var defaultProject = await _projectManager.GetDefaultProjectForCurrentTeam();

                vm.Projects = _projectManager.GetProjects(this._userAuthHelper.TeamId).Select(s => new ProjectVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsDefaultProject = (defaultProject != null && s.Id == defaultProject.Id)

                }).ToList();

            }
            catch (Exception e)
            {
                tc.TrackException(e);
            }

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
            var p = _projectManager.GetProject(id);
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
                    _projectManager.Save(model);
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
                tc.TrackException(ex);
                return Json(new { Status = "Error", Errors = new List<string> { "Error in saving project" } });
            }
        }

        public IActionResult Details(int id)
        {
            var project = _projectManager.GetProject(id);
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
            var issueCount = this._projectManager.GetIssueCountForProject(id);
            vm.DependableItemsCount = issueCount;
            return PartialView("Partial/DeleteConfirm", vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            try
            {
                this._projectManager.Delete(model.Id);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {
                tc.TrackException(ex);
                return Json(new { Status = "Error", Errors = new List<string> { "Error deleting project" } });
            }
        }


    }
}
