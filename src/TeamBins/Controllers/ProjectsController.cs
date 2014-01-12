using Planner.Controllers;
using Planner.DataAccess;
using Planner.Services;
using SmartPlan.DataAccess;
using System;
using System.Linq;
using System.Web.Mvc;
using TechiesWeb.TeamBins.ViewModels;

namespace SmartPlan.Controllers
{

    public class ProjectsController : BaseController
    {
        private IRepositary repo;
        private UserService userService;
        public ProjectsController()
        {
            repo = new Repositary();
            userService = new UserService(repo);
        }

        
        public ActionResult Index()
        {
            var vm = new TeamProjectListVM ();           
            var projectList = repo.GetProjects().Where(s => s.ProjectMembers.Any(b => b.UserID == UserID)).ToList();

            foreach (var project in projectList)
            {
                var projectVM = new ProjectVM { Name = project.Name, ID = project.ID };
               // projectVM.IsProjectOwner = true;
                vm.Projects.Add(projectVM);
            }
            return View(vm);
        }
        public ActionResult Details(int id)
        {
            var project = repo.GetProject(id);
            if (project != null)
            {
                var projectVm = new ProjectDetailsVM { ID = id, Name = project.Name };

                var projectMembers = project.ProjectMembers.ToList();
                foreach (var item in projectMembers)
                {
                    var member = new MemberVM { Name = item.Member.FirstName, JobTitle = item.Member.JobTitle };
                    projectVm.Members.Add(member);
                }
                return View(projectVm);
            }
            return View("NotFound");
        }
        public ActionResult Add()
        {
            var vm = new CreateProjectVM ();
            return PartialView("Partial/Add",vm);
        }

        [HttpPost]
        public ActionResult Add(CreateProjectVM model)
        {
            if (ModelState.IsValid)
            {
                var existing = repo.GetProject(model.Name,UserID);
                if((existing!=null) && (existing.ID!=model.ID))
                    return Json(new { Status="Error", Message= "Project name exists"});


                var project = new Project { Name = model.Name, ID=model.ID };
                project.CreatedByID = UserID;               
                var res=repo.SaveProject(project);
                if (res!=null)
                {
                    //Add as Project member
                    var projectMember = new ProjectMember { ProjectID = project.ID, UserID = UserID, CreatedDate = DateTime.Now };
                    var result = repo.SaveProjectMember(projectMember);

                    var projectCount = repo.GetProjects()
                                    .Where(s => s.ProjectMembers.Any(b => b.UserID == UserID)).Count();

                    //If this is the first project, then save as the default project
                    if (projectCount==1)
                    {
                        var defProjRes = userService.SaveDefaultIssueSettings(UserID, project.ID);    
                    }

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

            vm.DependableItemsCount = repo.GetIssues().Where(s => s.Project.ID == id).Count();

            return PartialView("Partial/DeleteConfirm",vm);
        }
        [HttpPost]
        public ActionResult DeleteConfirm(DeleteProjectConfirmVM model)
        {
            try
            {
                var result = repo.DeleteProject(model.ID);
                return Json(new { Status = "Success", Message = "Project deleted successfully" });
            }
            catch(Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error deleting project" });
            }
        }
       
        public ActionResult AddMember(int id)
        {
            var vm = new AddProjectMemberVM { ProjectID = id };
            return PartialView("Partial/AddMember",vm);
        }
        [HttpPost]
        public ActionResult AddMember(AddProjectMemberVM model)
        {
            try
            {
                var member=repo.GetUser(model.Email);
                if(member!=null)
                {
                    //Existing member, So lets add him to the project
                    var projectMember = new ProjectMember { ProjectID = model.ProjectID, UserID = member.ID, CreatedDate = DateTime.Now };
                    var result = repo.SaveProjectMember(projectMember);
                }
                return Json(new { Status = "Success", Message = "Project member added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error adding project member" });
            }
        }

        // JSON for auto complete
        public ActionResult Members(int id,string term)
        {
            //Returns project member list in JSON format
            var project= repo.GetProject(id);
            
            var projectMembers=project.ProjectMembers.Select(item => new { value = item.Member.FirstName, id = item.Member.ID.ToString() }).ToList();
            return Json( projectMembers , JsonRequestBehavior.AllowGet);
            
             
        }

    }
}
