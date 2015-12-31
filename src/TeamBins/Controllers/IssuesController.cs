using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StackExchange.Exceptional;
using TeamBins;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins.Helpers.Infrastructure;
using TeamBins.Services;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBins.ViewModels;
using TechiesWeb.TeamBins.ExtensionMethods;
namespace TechiesWeb.TeamBins.Controllers
{
    public class IssuesController : BaseController
    {
        ICommentManager commentManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        private IssueService issueService;
        IUserSessionHelper userSessionHelper;
        public IssuesController()
        {
            issueService = new IssueService(new Repositary(), UserID, TeamID);
        }

        public IssuesController(IRepositary repositary, IIssueManager issueManager, IProjectManager projectManager, IUserSessionHelper userSessionHelper, ICommentManager commentManager) : base(repositary)
        {
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.userSessionHelper = userSessionHelper;
            this.commentManager = commentManager;
        }

        #region public methods

        public JsonResult NonIssueMembers(string term, int issueId)
        {
            //Returns team members who are not assigned to the issue in a list in JSON format
            try
            {
                //TO DO : Have repo method to directly get non issue members

                var team = repo.GetTeam(TeamID);
                var list = new List<MemberVM>();
                var existingIssueMembers = repo.GetIssueMembers(issueId).ToList();

                var projectMembers = team.TeamMembers.Where(s => s.Member.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var member in projectMembers)
                {
                    if (!existingIssueMembers.Any(s => s.MemberID == member.MemberID))
                    {
                        var memberVM = new MemberVM { AvatarHash = UserService.GetAvatarUrl(member.Member.Avatar) };
                        memberVM.Name = member.Member.FirstName;
                        memberVM.MemberID = member.Member.ID;
                        list.Add(memberVM);
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json(new { Status = "Error", Message = "Troubles getting team members", JsonRequestBehavior.AllowGet });
            }
        }
        public ActionResult Index(int size = 50, string iteration = "current")
        {
            try
            {
                var statusIds = new List<int> { 1, 2, 3, 4 };
                IssueListVM bugListVM = new IssueListVM { TeamID = userSessionHelper.TeamId };
                var projectExists = projectManager.DoesProjectsExist();

                if (!projectExists)
                {
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    List<IssueVM> issueVMs = new List<IssueVM>();

                    if (Request.IsAjaxRequest())
                    {
                        issueVMs = issueManager.GetIssues(statusIds, 50).ToList();
                        return Json(issueVMs, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        bugListVM.Bugs = issueManager.GetIssues(statusIds, 50).ToList();
                        bugListVM.ProjectsExist = true;

                        bool defaultProjectExist = projectManager.GetDefaultProjectForCurrentTeam() > 0;
                        if (!defaultProjectExist)
                        {
                            var alertMessages = new AlertMessageStore();
                            alertMessages.AddMessage("system", String.Format("Hey!, You need to set a default project for the current team. Go to your <a href='{0}account/settings'>profile</a> and set a project as default project.", SiteBaseURL));
                            TempData["AlertMessages"] = alertMessages;
                        }
                        return View("Index", bugListVM);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorStore.LogException(ex, System.Web.HttpContext.Current);
                return View("Error");
            }
        }

        [HttpPost]
        [VerifyLogin]
        public ActionResult Add(CreateIssue model, List<HttpPostedFileBase> files)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousVersion = issueManager.GetIssue(model.Id);
                    var newVersion = issueManager.SaveIssue(model, files);
                    var issueActivity = issueManager.SaveActivity(model, previousVersion, newVersion);

                    var context = GlobalHost.ConnectionManager.GetHubContext<IssuesHub>();
                    if (issueActivity != null)
                    {
                        context.Clients.Group(TeamID.ToString()).addNewTeamActivity(issueActivity);
                        context.Clients.Group(TeamID.ToString()).addIssueToIssueList(newVersion);

                        //update the dashboard
                        var dashboardSummary = issueManager.GetDashboardSummaryVM(issueActivity.TeamId);

                        context.Clients.Group(TeamID.ToString()).updateDashboardSummary(dashboardSummary);
                    }
                    if (Request.IsAjaxRequest())
                    {
                        if (model.Id == 0)
                        {
                            //  var issueVM = issueService.GetIssueVM(issue);
                            // context.Clients.Group(TeamID.ToString()).addIssueToIssueList(issueVM);

                            //issueService.SendEmailNotificationsToSubscribers(issue, TeamID, UserID, SiteBaseURL);
                        }
                        return Json(new { Status = "Success" });
                    }

                    if ((files != null) && (files.Any()))
                    {
                        int fileCounter = 0;
                        foreach (var file in files)
                        {
                            // fileCounter = SaveAttachedDocument(model, result, fileCounter, file);
                        }
                    }


                    if (model.IsFromModalWindow)
                    {
                        if (Request.UrlReferrer != null)
                            return Redirect(Request.UrlReferrer.AbsoluteUri);
                    }
                    return RedirectToAction("Index");

                }
            }
            catch (MissingSettingsException mex)
            {
                log.Debug(mex);
                return Json(new { Status = "Error", Message = String.Format("You need to set a value for {0} first.", mex.MissingSettingName) });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Status = "Error", Message = "Error saving issue" });
                }
            }
            LoadDropDownsForCreate(model);
            return View(model);
        }

        [VerifyLogin]
        public ActionResult Edit(int id)
        {
            var bug = repo.GetIssue(id);
            if (bug != null)
            {
                var editVM = new CreateIssue();
                editVM.Title = bug.Title;
                editVM.Description = bug.Description;
                LoadDropDownsForCreate(editVM);

                editVM.SelectedCategory = bug.Category.ID;
                editVM.SelectedPriority = bug.Priority.ID;
                editVM.SelectedProject = bug.Project.ID;
                editVM.SelectedStatus = bug.Status.ID;
                editVM.SelectedIteration = bug.Location;
                editVM.CreatedDate = bug.CreatedDate;

                //var allDocuments = repo.GetDocuments(id, "Bug");
                /* var images = allDocuments.Where(x => x.Extension.ToUpper() == ".JPG" || x.Extension.ToUpper() == ".PNG");
                 foreach (var img in images)
                 {
                     var imgVM = new DocumentVM { FileName = img.DocName };
                     imgVM.FileKey = img.DocKey;
                     editVM.Images.Add(imgVM);
                 }*/

                if (Request.IsAjaxRequest())
                {
                    editVM.IsFromModalWindow = true;
                    return PartialView("~/Views/Issues/Partial/Edit.cshtml", editVM);
                }
                return View(editVM);
            }
            return View("NotFound");
        }

        [Route("issues/{id:int}")]
        [Route("issuecomment/{commentId}/{issuetitle}")]
        public ActionResult Details(int id = 0, int commentId = 0)
        {
            int issueId = 0;
            try
            {
                if (id > 0)
                {
                    issueId = id;
                }
                else if (id == 0 && commentId > 0)
                {
                    issueId = repo.GetComment(commentId).IssueID;
                }

                var bug = repo.GetIssue(issueId);

                if (bug == null || TeamID != bug.TeamID)
                    return View("NotFound");


                IssueDetailVM bugVm = new IssueDetailVM { Id = bug.ID, Title = bug.Title };

                bugVm = issueManager.GetIssue(id);

                //bugVm.Description = (bug.Description == null ? "" : bug.Description.ConvertUrlsToLinks());
                //bugVm.CreatedDate = bug.CreatedDate;
                //bugVm.OpenedBy = bug.CreatedBy.FirstName;
                //bugVm.Title = bug.Title;
                //bugVm.Project = bug.Project.Name;
                //bugVm.CategoryName = bug.Category.Name;
                //bugVm.ProjectID = bug.ProjectID;
                //bugVm.TeamID = bug.TeamID;
                //bugVm.StatusName = bug.Status.Name;
                //bugVm.PriorityName = bug.Priority.Name;
                //bugVm.StatusCode = bug.Status.Name;
                //if (bug.ModifiedDate.HasValue && bug.ModifiedDate.Value > DateTime.MinValue && bug.ModifiedBy != null)
                //{
                //    bugVm.LastModifiedDate = bug.ModifiedDate.Value.ToString("g");
                //    bugVm.LastModifiedBy = bug.ModifiedBy.FirstName;
                //}


                if (bug.DueDate.HasValue)
                    bugVm.IssueDueDate = (bug.DueDate.Value.Year > 2000 ? bug.DueDate.Value.ToShortDateString() : "");

                var allDocuments = repo.GetDocuments(issueId);
                foreach (var img in allDocuments)
                {
                    var imgVM = new DocumentVM { FileName = img.FileName, FileKey = img.FileAlias };
                    imgVM.FileExtn = Path.GetExtension(img.FileName);

                    if (imgVM.FileExtn.ToUpper() == ".JPG" || imgVM.FileExtn.ToUpper() == ".PNG")
                        bugVm.Images.Add(imgVM);
                    else
                        bugVm.Attachments.Add(imgVM);

                }

                //Get Members
                //issueService.LoadIssueMembers(issueId, bugVm, UserID);

                // issueService.SetUserPermissionsForIssue(bugVm, UserID, TeamID);
                return View(bugVm);
            }
            catch (Exception ex)
            {
                log.Error("error loading issue " + issueId, ex);
                return View("Error");
            }
        }

        [HttpPost]
        [VerifyLogin]
        public ActionResult RemoveMember(int id, int memberId)
        {
            try
            {
                repo.DeleteIssueMember(id, memberId);
                return Json(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json(new { Status = "Error", Message = "Error deleting issue member" });
            }
        }


        [HttpPost]
        [VerifyLogin]
        public JsonResult Star(int id, string mode)
        {
            //to do : Check user has permission to do this
            string starClass = "glyphicon-star-empty";
            string starMode = "unstarred";

            try
            {
                if (mode.ToUpper() == "UNSTARRED")
                {
                    issueService.StarIssue(id, UserID);
                    starClass = "glyphicon-star";
                    starMode = "starred";
                }
                else if (mode.ToUpper() == "STARRED")
                {
                    issueService.UnStarIssue(id, UserID);
                }
                return Json(new { Status = "Success", StarClass = starClass, Mode = starMode });
            }
            catch (Exception ex)
            {
                log.Error("Error staring issue " + id, ex);
                return Json(new { Status = "Error" });
            }

        }

        [HttpPost]
        public JsonResult AddMember(int memberId, int issueId)
        {
            try
            {
                issueService.SaveIssueMember(issueId, memberId, UserID);
                return Json(new { Status = "success" });
            }
            catch (Exception ex)
            {
                log.Error(string.Format("error saving member {0} for issue {1}", memberId, issueId), ex);
                return Json(new { Status = "error" });
            }
        }

        public JsonResult Members(int id)
        {
            var issueMembers = repo.GetIssueMembers(id);
            var list = new List<dynamic>();
            if (issueMembers != null)
            {
                foreach (var item in issueMembers)
                {
                    var memberVM = new { MemberID = item.MemberID, Name = item.Member.FirstName, AvatarHash = UserService.GetAvatarUrl(item.Member.Avatar), ShowDelete = false };
                    list.Add(memberVM);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssueMembers(int id)
        {
            var vm = new IssueDetailVM { Id = id };
            issueService.LoadIssueMembers(id, vm, UserID);
            issueService.SetUserPermissionsForIssue(vm, UserID, TeamID);
            return PartialView("Partial/Members", vm);
        }

        [HttpPost]
        [VerifyLogin]
        public async Task<ActionResult> Comment(NewIssueCommentVM model, string Connection)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    model.CommentBody = HttpUtility.HtmlEncode(model.CommentBody);
                    var comment = new CommentVM
                    {
                        CommentBody = model.CommentBody,
                        IssueId = model.IssueID,
                        Author = new UserDto { Id = UserID }
                    };

                    var commentId = commentManager.SaveComment(comment);

                    var newCommentVm = commentManager.GetComment(commentId);
                    var context = GlobalHost.ConnectionManager.GetHubContext<IssuesHub>();

                    //Send to all other users except the Author.
                    newCommentVm.IsOwner = false;
                    string[] excludedConn = new string[] { Connection };
                    context.Clients.Groups(new string[] { TeamID.ToString() }, excludedConn).addNewComment(newCommentVm);

                    //For Sending to the author
                    newCommentVm.IsOwner = true;
                    
                    await commentManager.SendEmailNotificaionForNewComment(comment);

                    return Json(new { Status = "Success", Data = newCommentVm });
                }
                return Json(new { Status = "Error" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json(new { Status = "Error" });
            }
        }

        public ActionResult Delete(int id)
        {
            var deleteConfirmVM = new DeleteIssueConfirmationVM { Id = id };
            return PartialView("Partial/DeleteConfirm", deleteConfirmVM);
        }
        [HttpPost]
        [VerifyLogin]
        public ActionResult Delete(int id, string token = "")
        {
            // to do : Check user permission
            try
            {
                var result = repo.DeleteIssue(id);
                if (result.Status)
                    return Json(new { Status = "Success" });
                else
                    log.Debug(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return Json(new { Status = "Error" });
        }

        [HttpPost]
        [VerifyLogin]
        public void SaveDueDate(string issueDueDate, int issueId)
        {
            try
            {
                bool isValidDate = false;
                var issue = repo.GetIssue(issueId);
                if (issue != null)
                {
                    if (!String.IsNullOrEmpty(issueDueDate))
                    {
                        issue.DueDate = DateTime.Parse(issueDueDate);
                        isValidDate = true;
                    }
                    else
                    {
                        issue.DueDate = DateTime.Now.AddYears(-1000);
                    }

                    var result = repo.SaveIssue(issue);
                    if (result.Status)
                    {
                        if (isValidDate)
                        {
                            var activity = issueService.SaveActivityForDueDate(issueId, TeamID, UserID);
                            if (activity != null)
                            {
                                var activityVM = issueService.GetActivityVM(activity);
                                var context = GlobalHost.ConnectionManager.GetHubContext<IssuesHub>();
                                context.Clients.Group(TeamID.ToString()).addNewTeamActivity(activityVM);
                            }
                        }
                    }
                    else
                    {
                        log.Error(result);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }





        #endregion public methods

        #region private methods

        


        private void LoadDropDownsForCreate(CreateIssue viewModel)
        {
            viewModel.Projects = projectManager.GetProjects().Select(s=>new SelectListItem { Value = s.Id.ToString(), Text =s.Name}).ToList();
            viewModel.Priorities = ProjectService.GetPriorities(repo);
            viewModel.Categories = ProjectService.GetCategories(repo);
            viewModel.Statuses = ProjectService.GetStatuses(repo);
            viewModel.Iterations = ProjectService.GetIterations();
        }

        private int SaveAttachedDocument(CreateIssue model, OperationStatus result, int fileCounter, HttpPostedFileBase file)
        {
            if (file != null)
            {
                fileCounter++;

                string fileName = Path.GetFileName(file.FileName).ToLower();
                string fileKey = fileName;
                fileKey = fileKey.Replace(" ", "-").Replace("%", "-");

                fileKey = String.Format("{0}-{1}-{2:n}-{3}", result.OperationID, fileCounter, Guid.NewGuid(), fileName);

                if (fileKey.Length > 99)
                    fileKey = fileKey.Substring(0, 99);

                string path = Path.Combine(Server.MapPath("~/uploads"), fileKey);
                file.SaveAs(path);

                Document img = new Document { FileName = fileName, ParentID = model.Id };
                img.FileAlias = fileKey;
                img.CreatedByID = UserID;
                img.ParentID = result.OperationID;
                var resultForImg = repo.SaveDocument(img);
                if (!resultForImg.Status)
                    log.Debug(resultForImg);
            }
            return fileCounter;
        }

        private void LoadDefaultIssueValues(Issue issue, CreateIssue model)
        {
            issue.PriorityID = model.SelectedPriority;
            if (issue.PriorityID == 0)
                issue.PriorityID = 1;

            issue.ProjectID = model.SelectedProject;
            if (issue.ProjectID == 0)
            {
                var teamMember = repo.GetTeamMember(UserID, TeamID);
                if (teamMember.DefaultProjectID == null)
                    throw new MissingSettingsException("Default Project not set", "Default Project");
                //get from team member 
                issue.ProjectID = teamMember.DefaultProjectID.Value;
            }
            issue.StatusID = model.SelectedStatus;
            if ((model.Id == 0) && (model.SelectedStatus == 0))
            {
                issue.StatusID = 1;
            }
            issue.CategoryID = model.SelectedCategory;
            if (issue.CategoryID == 0)
                issue.CategoryID = 1;

            issue.Location = (string.IsNullOrEmpty(model.SelectedIteration) ? "SPRNT" : model.SelectedIteration);

        }

        private Activity SaveActivity(CreateIssue model, Issue existingIssue, Issue issuePreviousVersion, Status currentIssueStatus, int issueId, bool isNewIssue = false)
        {
            bool isStateChanged = false;
            var activity = new Activity() { CreatedByID = UserID, ObjectID = issueId, ObjectType = "Issue" };

            if (isNewIssue)
            {
                activity.ActivityDesc = "Created";
                activity.NewState = model.Title;
                isStateChanged = true;
            }
            else
            {
                if (issuePreviousVersion.StatusID != existingIssue.StatusID)
                {
                    // status of issue updated
                    activity.OldState = model.Title;
                    activity.ActivityDesc = "Changed status";
                    activity.NewState = currentIssueStatus.Name;
                    isStateChanged = true;
                }

            }

            activity.TeamID = TeamID;

            if (isStateChanged)
            {
                var result = repo.SaveActivity(activity);
                if (!result.Status)
                {
                    log.Error(result);
                }
                return repo.GetActivity(activity.ID);


            }
            return null;
        }


        #endregion private methods

    }
}



