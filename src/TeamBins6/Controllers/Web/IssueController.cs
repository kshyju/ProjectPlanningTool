using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;
using TeamBins6.Infrastrucutre;
using TeamBins6.Infrastrucutre.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamBins6.Controllers.Web
{
    public class IssueController : Controller
    {
        ICommentManager commentManager;
        private readonly IProjectManager projectManager;
        private IIssueManager issueManager;
        //private IssueService issueService;
        IUserSessionHelper userSessionHelper;
       

        public IssueController(ICommentManager commentManager, IUserSessionHelper userSessionHelper, IProjectManager projectManager, IIssueManager issueManager) //: base(repositary)
        {
            this.issueManager = issueManager;
            this.projectManager = projectManager;
            this.userSessionHelper = userSessionHelper;
            this.commentManager = commentManager;
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

                    bool defaultProjectExist = projectManager.GetDefaultProjectForCurrentTeam() !=null;
                    if (!defaultProjectExist)
                    {
                        var alertMessages = new AlertMessageStore();
                       // alertMessages.AddMessage("system", String.Format("Hey!, You need to set a default project for the current team. Go to your <a href='{0}account/settings'>profile</a> and set a project as default project.", SiteBaseURL));
                        TempData["AlertMessages"] = alertMessages;
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


        [HttpPost]

        public ActionResult Add(CreateIssue model, List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousVersion = issueManager.GetIssue(model.Id);
                    var newVersion = issueManager.SaveIssue(model, files);
                  //  var issueActivity = issueManager.SaveActivity(model, previousVersion, newVersion);

                    if ((files != null) && (files.Any()))
                    {
                        int fileCounter = 0;
                        foreach (var file in files)
                        {
                            // fileCounter = SaveAttachedDocument(model, result, fileCounter, file);
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error saving issue" });
            }
          

            return View(model);
        }
    }
}
