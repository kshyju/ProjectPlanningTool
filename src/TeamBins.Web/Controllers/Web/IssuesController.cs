using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins.Infrastrucutre;
using TeamBins.Infrastrucutre.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
using TeamBins.Common;


namespace TeamBins.Controllers.Web
{
    public class IssuesController : BaseController
    {
        private readonly ITeamManager teamManager;


        private readonly IProjectManager projectManager;
        private readonly IIssueManager issueManager;
        readonly IUserAuthHelper userSessionHelper;
        private readonly IUploadHandler uploadHandler;
        private readonly IUploadManager uploadManager;
        private readonly IUrlHelper urlHelper;
        public IssuesController(IUserAuthHelper userSessionHelper,
            IProjectManager projectManager, IIssueManager issueManager,
            ITeamManager teamManager, IUploadHandler uploadHandler,
            IUploadManager uploadManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) //: base(repositary)
        {
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.userSessionHelper = userSessionHelper;
            this.teamManager = teamManager;
            this.uploadHandler = uploadHandler;
            this.uploadManager = uploadManager;
            this.urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

        }

        [Route("Issues")]
        [Route("Issues/{teamId}/{teamName}")]

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



        [Route("Issues/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var vm = new IssueDetailVM();
            vm = await this.issueManager.GetIssue(id);
            if (vm != null && vm.Active)
            {
                vm.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId, this.userSessionHelper.TeamId);
                vm.IsReadOnly = this.userSessionHelper.UserId == 0;
                return View(vm);
            }
            return View("NotFound");
        }

        [Route("Issues/add")]
        public async Task<IActionResult> Add()
        {

            var vm = new CreateIssue();
            this.issueManager.LoadDropdownData(vm);

            return PartialView("~/Views/Issues/Partial/Edit.cshtml", vm);
        }

        [Route("Issues/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this.issueManager.GetIssue(id);
            if (issue != null && issue.Active)
            {
                var vm = new CreateIssue(issue);
                this.issueManager.LoadDropdownData(vm);

                vm.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId, this.userSessionHelper.TeamId);
                return PartialView("~/Views/Issues/Partial/Edit.cshtml", vm);
            }
            return PartialView("NotFound");
        }

        [HttpPost]
        [Route("Issues/QuickAdd")]
        public async Task<IActionResult> QuickAdd([FromBody] CreateIssue model)
        {
            var result = await Add(model);
            return result;
        }

        [HttpPost]
        [Route("Issues/Add")]
        public async Task<IActionResult> Add(CreateIssue model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousVersion = await issueManager.GetIssue(model.Id);
                    var newVersion = await issueManager.SaveIssue(model, null);
                    var issueActivity = issueManager.SaveActivity(model, previousVersion, newVersion);

                    //ConnectionManager c = new ConnectionManager(new DefaultDependencyResolver());
                    //  var context = c.GetHubContext<IssuesHub>();
                    // context.Clients.All.addNewTeamActivity(issueActivity);

                    if ((model.Files != null) && (model.Files.Any()))
                    {
                        foreach (var file in model.Files)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            using (var s = file.OpenReadStream())
                            {
                                var uploadResult = await uploadHandler.UploadFile(fileName, MimeMapping.GetMimeMapping(fileName), s);
                                if (!String.IsNullOrEmpty(uploadResult.Url))
                                {
                                    uploadResult.ParentId = model.Id;
                                    uploadResult.CreatedById = this.userSessionHelper.UserId;
                                    uploadResult.Type = "Issue";
                                    await this.uploadManager.SaveUpload(uploadResult);
                                }
                            }
                        }

                    }
                    if (previousVersion != null)
                    {
                        
                    }
                    if (model.IncludeIssueInResponse)
                    {
                        var newIssue = issueManager.GetIssue(newVersion.Id);
                        return Json(new { Status = "Success", Data = newIssue });
                    }

                    var newIssueUrl = this.urlHelper.Action("Details", new { id = newVersion.Id });
                    return Json(new { Status = "Success", Url = newIssueUrl });
                }
                else
                {
                    var validationErrors = new List<string>();
                    foreach (var modelStateVal in ViewData.ModelState.Values)
                    {
                        validationErrors.AddRange(modelStateVal.Errors.Select(error => error.ErrorMessage));
                    }

                    return Json(new { Status = "Error", Message = "Validation failed", Errors = validationErrors });
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

        [Route("Issues/delete/{id}")]
        public IActionResult Delete(int id)
        {
            var deleteConfirmVM = new DeleteIssueConfirmationVM { Id = id };
            return PartialView("Partial/DeleteConfirm", deleteConfirmVM);
        }


        [Route("Issues/{id}/Members")]
        public async Task<IActionResult> Members(int id)
        {
            var issue = new IssueDetailVM();
            issue.IsEditableForCurrentUser = this.teamManager.DoesCurrentUserBelongsToTeam(this.userSessionHelper.UserId, this.userSessionHelper.TeamId);
            var members = await issueManager.GetIssueMembers(id);
            issue.Members = members.Select(x => x.Member);
            return PartialView("Partial/Members", issue);
        }

    }
}
