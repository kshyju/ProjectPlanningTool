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
    public class IssueController : BaseController
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

         [Route("Issue")]
        [Route("Issue/{teamId}/{teamName}")]
       
        public ActionResult Index(int? teamId, string teamName = "")
        {
            try
            {
                var teamIdToUse = userSessionHelper.TeamId;
                var bugListVm = new IssueListVM();

                if (teamId != null)
                {
                    teamIdToUse = teamId.Value;
                    // a publicily visible Issue board
                    var team = this.teamManager.GetTeam(teamId.Value);
                    if (team != null && team.IsPublic)
                    {
                        bugListVm.TeamID = team.Id;
                        bugListVm.IsPublicTeam = true;
                        bugListVm.ProjectsExist = true;
                        bugListVm.IsReadonlyView = true;

                    }
                    else
                    {
                        teamIdToUse = 0;
                    }
                }

                bugListVm.TeamID = teamIdToUse;



                if (!bugListVm.IsReadonlyView)
                {
                    var projectExists = projectManager.DoesProjectsExist();

                    if (!projectExists)
                    {
                        return RedirectToAction("Index", "Projects");
                    }
                }
                bugListVm.ProjectsExist = true;




                bool defaultProjectExist = projectManager.GetDefaultProjectForCurrentTeam() != null;
                if (!defaultProjectExist)
                {
                    var tt = new Dictionary<string, string>
                        {
                            {
                                "warning",
                                "Hey!, You need to set a default project for the current team. Go to your profile settings and set a project as default project."
                            }
                        };
                    TempData["AlertMessages"] = tt;
                }



                return View("Index", bugListVm);

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
                vm.IsReadOnly = this.userSessionHelper.UserId == 0;
                return View(vm);
            }
            return View("NotFound");
        }

        [Route("Issue/edit/{id}")]
        public IActionResult Edit(int id)
        {
            var issue = this.issueManager.GetIssue(id);
            if (issue != null && issue.Active)
            {
                var vm = new CreateIssue(issue);
                this.issueManager.LoadDropdownData(vm);

                vm.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam();
                return PartialView("~/Views/Issue/Partial/Edit.cshtml", vm);
            }
            return PartialView("NotFound");
        }

        [HttpPost]
        [Route("Issue/Add")]
        public async Task<IActionResult> Add([FromBody] CreateIssue model) //, List<IFormFile> files
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousVersion = issueManager.GetIssue(model.Id);
                    var newVersion = await issueManager.SaveIssue(model, null);
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
                        return Json(new { Status = "Success", Data = newIssue });
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

        [Route("Issue/delete/{id}")]
        public IActionResult Delete(int id)
        {
            var deleteConfirmVM = new DeleteIssueConfirmationVM { Id = id };
            return PartialView("Partial/DeleteConfirm", deleteConfirmVM);
        }


        [Route("Issue/{id}/Members")]
        public async Task<IActionResult> Members(int id)
        {
            var issue = new IssueDetailVM();
            issue.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam();
            var members = await issueManager.GetIssueMembers(id);
            issue.Members = members;



            return PartialView("Partial/Members", issue);
        }



    }
}
