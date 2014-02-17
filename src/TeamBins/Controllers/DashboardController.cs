using SmartPlan.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TeamBins.Helpers.Enums;
using TeamBins.Services;
using TechiesWeb.TeamBins.ViewModels;
namespace TechiesWeb.TeamBins.Controllers
{
    public class DashboardController : BaseController
    {        
        public DashboardController()
        {
            repo=new Repositary();            
        }
        public DashboardController(IRepositary repositary) : base(repositary)
        {           

        }
      
        public ActionResult Index(int? teamid,string teamname = "")
        {
            try
            {
                if (teamid.HasValue)
                {
                    //User switched team from the header menu
                    UpdateTeam(teamid.Value);
                }
                var vm = new DashBoardVM { TeamID = TeamID };
                var projectList = repo.GetProjects(TeamID).Where(s => s.TeamID == TeamID).ToList();
                foreach (var project in projectList)
                {
                    var projectVM = new ProjectVM { ID = project.ID, Name = project.Name, Description = project.Description };
                    projectVM.IsProjectOwner = (project.CreatedByID == UserID);
                    vm.Projects.Add(projectVM);
                }
                vm.RecentIssues = GetRecentIssues();
                vm.IssuesAssignedToMe = GetIssuesAssignedToMe();

                return View(vm);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return View("Error");
            }
        }

        public JsonResult GetDashBoardItemSummary()
        {
            var issueService = new IssueService(repo, UserID, TeamID);
            var vm = issueService.GetDashboardSummaryVM(TeamID);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }



        private List<IssueVM> GetRecentIssues()
        {
            var listIssues = new List<IssueVM>();

            var issueList = repo.GetIssues(TeamID).OrderByDescending(s => s.ID).Take(5);
            foreach (var issue in issueList)
            {
                var issueVM = new IssueVM { ID = issue.ID, Title = issue.Title };
                listIssues.Add(issueVM);
            }
            return listIssues;
        }

        private List<IssueVM> GetIssuesAssignedToMe()
        {
            //Gets the issues assigned to the current user
            var listIssues = new List<IssueVM>();

            var issueList = repo.GetIssues(TeamID).Where(s => s.IssueMembers.Any(f => f.MemberID == UserID))
                .OrderByDescending(x=>x.ID).Take(5);
            foreach (var issue in issueList)
            {
                var issueVM = new IssueVM { ID = issue.ID, Title = issue.Title };
                listIssues.Add(issueVM);
            }
            return listIssues;
        }

    }
}
