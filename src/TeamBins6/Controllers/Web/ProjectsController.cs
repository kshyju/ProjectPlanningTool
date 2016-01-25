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
        // GET: /<controller>/
        public IActionResult Index()
        {
            var vm = new TeamProjectListVM();
            return View(vm);
        }

        public IActionResult Add()
        {
            var addVm = new CreateProjectVM();
            return PartialView("Partial/Add", addVm);
        }

        [HttpPost]
        public ActionResult Add(CreateProjectVM model)
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


    }
}
