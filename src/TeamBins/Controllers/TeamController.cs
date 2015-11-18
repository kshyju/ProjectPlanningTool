using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TeamBins.Common;
using TeamBins.DataAccess;
using TeamBins.Helpers.Enums;
using TeamBins.Services;
using TechiesWeb.TeamBins.ViewModels;
namespace TechiesWeb.TeamBins.Controllers
{
   // [VerifyLogin]
    public class TeamController : BaseController
    {
        ITeamManager teamManager;
       private IssueService issueService;
        public TeamController() {
            issueService=new IssueService(new Repositary(),UserID,TeamID);
        
        }

        public TeamController(IRepositary repositary, ITeamManager teamManager)
            : base(repositary)
        {
            this.teamManager = teamManager;
        }

     
        public ActionResult Index()
        {
            try
            {
                var teamListVm = new TeamListVM {Teams = teamManager.GetTeams()};

                return View(teamListVm);
            }
            catch (Exception ex)
            {
                log.Error("error loading teams for user " + UserID, ex);
                return View("Error");                    
            }
        }
       
        public ActionResult View(int id)        
        {
            
            var team = repo.GetTeam(id);
            if (team != null)
            {
                var vm = new TeamVM { Id = id, Name = team.Name };

              /*  var teamMembers = team.TeamMembers;
                foreach (var teamMember in teamMembers)
                {
                   var member=new MemberVM { Name = teamMember.User.FirstName + " " + teamMember.User.LastName, EmailAddress=teamMember.User.EmailAddress, JobTitle=teamMember.JobTitle, JoinedDate = teamMember.CreatedDate.ToShortDateString() };
                   member.EmailHash = UserService.GetImageSource(member.EmailAddress);
                   vm.Members.Add(member);
                }*/
       
                return View(vm);
            }
            return View("NotFound");
           
        }/*
        public ActionResult Create()
        {
            return View(new TeamVM());
        }*/
        public ActionResult Add()
        {
            return PartialView("Edit", new TeamVM());
        }
        public ActionResult Edit(int id)
        {
             var team = repo.GetTeam(id);
             if (team != null)
             {
                 var vm = new TeamVM { Name = team.Name, Id = team.ID }; 
                 return PartialView(vm);
             }
             return View("NotFound");
        }
        public JsonResult NonTeamMembers(string term)
        {
            var nonMembers = repo.GetNonTeamMemberUsers(TeamID,term)
                 .Where(s => s.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase) == true)
                 .ToList();

            var nonTeamMembers = nonMembers                
                 .Select(x => new MemberVM { Name = x.FirstName, AvatarHash = UserService.GetAvatarUrl( x.Avatar), MemberID = x.ID })
                 .ToList();

            return Json(nonTeamMembers, JsonRequestBehavior.AllowGet);
        }
        /*
        public ActionResult AddMember(int id)
        {
            var vm=new AddTeamMemberRequestVM { TeamID=id};
            return View(vm);
        }
        public ActionResult MemberInvited()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMember(AddTeamMemberRequestVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var team = new TeamMemberRequest { EmailAddress = model.EmailAddress, CreatedByID = UserID };
                    team.TeamID = model.TeamID;
                    team.ActivationCode = model.TeamID + "-" + Guid.NewGuid().ToString("n");
                   /* var result = repo.AddTeamMemberRequest(team);
                    if (result != null)
                    {
                        return RedirectToAction("memberinvited", "Team");
                    }
                    else
                    {
                        //LOG ERROR
                    }*//*
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);

            }

        }
        */
        [HttpPost]
        public ActionResult Create(TeamVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Team team = new Team { Name = model.Name, ID = model.Id };
                    bool isNew = (model.Id == 0);
                    if (!isNew)
                    {
                        team = repo.GetTeam(model.Id);
                        team.Name = model.Name;                       
                    }
                    else
                    {
                        team.CreatedByID = UserID;
                    }
                    var result = repo.SaveTeam(team);
                    if (result != null)
                    {                        
                        if (isNew)
                        {
                            var teamMember = new TeamMember { MemberID = UserID, TeamID = team.ID, CreatedByID = UserID };
                            repo.SaveTeamMember(teamMember);
                        }
                        return Json(new { Status = "Success" });
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                log.Error("Error updating team "+model.Id, ex);
            }
            return Json(new { Status = "Error" });
        }    



        // we will move this method to the web api once authentication stuff is finalized.
        public ActionResult Stream(int id,int size=25)
        {
            List<ActivityVM> list = new List<ActivityVM>();
            list = GetTeamActivityVMs(id,size);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
       
        private List<ActivityVM> GetTeamActivityVMs(int teamId, int size)
        {
            List<ActivityVM> activityVMList = new List<ActivityVM>();
            try
            {
                var activityList = repo.GetTeamActivity(teamId).OrderByDescending(s => s.CreatedDate).Take(size).ToList();

                ActivityVM activityVM = new ActivityVM();
                var issueService = new IssueService(repo, UserID, TeamID);
                issueService.SiteBaseURL = SiteBaseURL;
                var commentService = new CommentService(repo, SiteBaseURL);

                foreach (var item in activityList)
                {
                    if (item.ObjectType == ActivityObjectType.Issue.ToString())
                    {
                        activityVM = issueService.GetActivityVM(item);
                    }
                    else if (item.ObjectType == ActivityObjectType.IssueComment.ToString())
                    {
                        activityVM = commentService.GetActivityVM(item);
                    }
                    activityVMList.Add(activityVM);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return activityVMList;
        }


    }
}
