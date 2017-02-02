using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins.Infrastrucutre;
using TeamBins.Infrastrucutre.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using TeamBins.Common;


namespace TeamBins.Controllers.Web
{
    public class IssuesController : BaseController
    {
        private readonly ITeamManager _teamManager;


        private readonly IProjectManager _projectManager;
        private readonly IIssueManager _issueManager;
        readonly IUserAuthHelper _userSessionHelper;
        private readonly IUploadHandler _uploadHandler;
        private readonly IUploadManager _uploadManager;
        
        private readonly IUrlHelper _urlHelper;
       
        public IssuesController(IUserAuthHelper userSessionHelper,
            IProjectManager projectManager, IIssueManager issueManager,
            ITeamManager teamManager, IUploadHandler uploadHandler,
            IUploadManager uploadManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IOptions<AppSettings> settings) : base(settings)
        {
            this._issueManager = issueManager;
            this._projectManager = projectManager;
            this._userSessionHelper = userSessionHelper;
            this._teamManager = teamManager;
            this._uploadHandler = uploadHandler;
            this._uploadManager = uploadManager;
           
            this._urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
          
        }

        [Route("Issues")]
        [Route("Issues/{teamId}/{teamName}")]

        public ActionResult Index(int? teamId, string teamName = "")
        {
            var tc = new Microsoft.ApplicationInsights.TelemetryClient();
            tc.TrackEvent("Issue list view");

            try
            {
                var teamIdToUse = _userSessionHelper.TeamId;


                var bugListVm = new IssueListVM();

                if (teamId != null)
                {
                    teamIdToUse = teamId.Value;
                    // a publicily visible Issue board
                    var team = this._teamManager.GetTeam(teamId.Value);
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
                    var projectExists = _projectManager.DoesProjectsExist();

                    if (!projectExists)
                    {
                        return RedirectToAction("Index", "Projects");
                    }
                }
                bugListVm.ProjectsExist = true;




                bool defaultProjectExist = _projectManager.GetDefaultProjectForCurrentTeam() != null;
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
            vm = await this._issueManager.GetIssue(id);
            if (vm != null && vm.Active)
            {
                vm.IsEditableForCurrentUser = this._teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, this._userSessionHelper.TeamId);
                vm.IsReadOnly = this._userSessionHelper.UserId == 0;
                return View(vm);
            }
            return View("NotFound");
        }

        [Route("Issues/add")]
        public async Task<IActionResult> Add()
        {

            var vm = new CreateIssue();
            this._issueManager.LoadDropdownData(vm);

            return PartialView("~/Views/Issues/Partial/Edit.cshtml", vm);
        }

        [Route("Issues/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this._issueManager.GetIssue(id);
            if (issue != null && issue.Active)
            {
                var vm = new CreateIssue(issue);
                this._issueManager.LoadDropdownData(vm);

                vm.IsEditableForCurrentUser = this._teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, this._userSessionHelper.TeamId);
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
                    var previousVersion = await _issueManager.GetIssue(model.Id);
                    var newVersion = await _issueManager.SaveIssue(model, null);
                    var issueActivity = _issueManager.SaveActivity(model, previousVersion, newVersion);

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
                                var uploadResult = await _uploadHandler.UploadFile(fileName, MimeMapping.GetMimeMapping(fileName), s);
                                if (!String.IsNullOrEmpty(uploadResult.Url))
                                {
                                    uploadResult.ParentId = model.Id;
                                    uploadResult.CreatedById = this._userSessionHelper.UserId;
                                    uploadResult.Type = "Issue";
                                    await this._uploadManager.SaveUpload(uploadResult);
                                }
                            }
                        }

                    }
                    if (previousVersion != null)
                    {
                        
                    }
                    if (model.IncludeIssueInResponse)
                    {
                        var newIssue = _issueManager.GetIssue(newVersion.Id);
                        return Json(new { Status = "Success", Data = newIssue });
                    }

                    var newIssueUrl = this._urlHelper.Action("Details", new { id = newVersion.Id });
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
            issue.IsEditableForCurrentUser = this._teamManager.DoesCurrentUserBelongsToTeam(this._userSessionHelper.UserId, this._userSessionHelper.TeamId);
            var members = await _issueManager.GetIssueMembers(id);
            issue.Members = members.Select(x => x.Member);
            return PartialView("Partial/Members", issue);
        }

    }
}
