using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Newtonsoft.Json;
using StackExchange.Exceptional;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Common.Infrastructure.Exceptions;
using TeamBins6.Infrastrucutre;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class IssueController : Controller
    {
        private ITeamManager teamManager;
        ICommentManager commentManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        //private IssueService issueService;
        IUserSessionHelper userSessionHelper;
        

        public IssueController(ICommentManager commentManager, IUserSessionHelper userSessionHelper, IProjectManager projectManager, IIssueManager issueManager, ITeamManager teamManager) //: base(repositary)
        {
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.userSessionHelper = userSessionHelper;
            this.commentManager = commentManager;
            this.teamManager = teamManager;
        }


        public ActionResult Index(int size = 50, string iteration = "current")
        {
            try
            {
                IssueListVM bugListVM = new IssueListVM { TeamID = userSessionHelper.TeamId };
                var projectExists = projectManager.DoesProjectsExist();

                if (!projectExists)
                {
                    return RedirectToAction("Index", "Projects");
                }
                else
                {

                    bugListVM.ProjectsExist = true;

                    bool defaultProjectExist = projectManager.GetDefaultProjectForCurrentTeam() != null;
                    if (!defaultProjectExist)
                    {
                        var some = new TestClass { Name = "Tes" };
                        var alertMessages = new AlertMessageStore();

                        // alertMessages.AddMessage("system", String.Format("Hey!, You need to set a default project for the current team. Go to your <a href='{0}account/settings'>profile</a> and set a project as default project.",""));
                        //TempData["AlertMessages"] = some; //alertMessages;// "alertMessages";
                    }
                    return View("Index", bugListVM);

                }

            }
            catch (Exception ex)
            {
                //ErrorStore.LogException(ex, System.Web.HttpContext.Current);
                return View("Error");
            }
        }

       

        [Route("Issue/{id}")]
        public IActionResult Details(int id)
        {
            var vm = new IssueDetailVM();
            vm = this.issueManager.GetIssue(id);
            if (vm != null && vm.Active)
            {
                vm.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam();

                return View(vm);
            }
            return View("NotFound");
        }

        public IActionResult Edit(int id)
        {
            var issue = this.issueManager.GetIssue(id);
            if (issue != null && issue.Active)
            {
                var vm = new CreateIssue(issue);
                this.issueManager.LoadDropdownData(vm);

               


                vm.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam();
                return PartialView("~/Views/Issue/Partial/Edit.cshtml",vm);
            }
            return PartialView("NotFound");
        }

        [HttpPost]
        [Route("Issue/Add")]
        public ActionResult Add([FromBody] CreateIssue model) //, List<IFormFile> files
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousVersion = issueManager.GetIssue(model.Id);
                    var newVersion = issueManager.SaveIssue(model, null);
                    var issueActivity = issueManager.SaveActivity(model, previousVersion, newVersion);

                    //if ((files != null) && (files.Any()))
                    //{
                    //    int fileCounter = 0;
                    //    foreach (var file in files)
                    //    {
                    //        // fileCounter = SaveAttachedDocument(model, result, fileCounter, file);
                    //    }
                    //}
                    if (model.IncludeIssueInResponse)
                    {
                        var newIssue = issueManager.GetIssue(newVersion.Id);
                        return Json(new { Status = "Success" , Data = newIssue});
                    }
                    
                    return Json(new { Status = "Success" });
                }
            }
            catch (MissingSettingsException mex)
            {
                return Json(new { Status = "Error", Message = String.Format("You need to set a value for {0} first.", mex.MissingSettingName) });
            }
            catch (Exception ex)
            {
                //  bErrorStore.LogException(ex, Request.HttpContext);

                return Json(new { Status = "Error", Message = "Error saving issue" });
            }


            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var deleteConfirmVM = new DeleteIssueConfirmationVM { Id = id };
            return PartialView("Partial/DeleteConfirm", deleteConfirmVM);
        }





    }
}
