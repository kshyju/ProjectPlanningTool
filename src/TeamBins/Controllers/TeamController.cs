using System.Collections.Generic;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TeamBins.Services;
using TechiesWeb.TeamBins.ViewModels;
using System.Linq;
using System;
namespace TechiesWeb.TeamBins.Controllers
{

    public class TeamController : BaseController
    {       
       private IssueService issueService;
        public TeamController() {
            issueService=new IssueService(new Repositary());
        
        }

        public TeamController(IRepositary repositary)
            : base(repositary)
        {            

        }

     
        public ActionResult Index()
        {
            var teams = repo.GetTeams(UserID);
            var teamListVM = new TeamListVM();
            foreach (var team in teams)
            {
                teamListVM.Teams.Add(new TeamVM { ID = team.ID, Name = team.Name , IsTeamOwner=team.CreatedByID==UserID });
            }
            return View(teamListVM);
        }
         /*
        public ActionResult View(int id)        
        {
            
            var team = repo.GetTeam(id);
            if (team != null)
            {
                var vm = new TeamVM { ID = id, Name = team.Name };

              /*  var teamMembers = team.TeamMembers;
                foreach (var teamMember in teamMembers)
                {
                   var member=new MemberVM { Name = teamMember.User.FirstName + " " + teamMember.User.LastName, EmailAddress=teamMember.User.EmailAddress, JobTitle=teamMember.JobTitle, JoinedDate = teamMember.CreatedDate.ToShortDateString() };
                   member.EmailHash = UserService.GetImageSource(member.EmailAddress);
                   vm.Members.Add(member);
                }*/
        /*
                return View(vm);
            }
            return View("NotFound");
           
        }
        public ActionResult Create()
        {
            return View(new TeamVM());
        }*/
        public ActionResult Edit(int id)
        {
             var team = repo.GetTeam(id);
             if (team != null)
             {
                 var vm = new TeamVM { Name = team.Name, ID = team.ID };
                 return PartialView(vm);
             }
             return View("NotFound");
        }/*
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
                    Team team = new Team { Name = model.Name, ID = model.ID };
                    bool isNew = (model.ID == 0);
                    if (!isNew)
                    {
                        team = repo.GetTeam(model.ID);
                        team.Name = model.Name;
                    }
                    var result = repo.SaveTeam(team);
                    if (result != null)
                    {
                        return Json(new { Status = "Success" });
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                log.Error("Error updating team "+model.ID,ex);
            }
            return Json(new { Status = "Error" });
        }
        /*
        public ActionResult Created()
        {
            string strTeamId = TempData["TeamID"].ToString();
            if (!String.IsNullOrEmpty(strTeamId))
            {
                int teamId = Convert.ToInt32(strTeamId);
                var team = repo.GetTeam(teamId);
                var teamVM = new TeamVM { Name = team.Name  , ID=teamId};
                return View(teamVM);
            }
            return RedirectToAction("Index", "Dashboard");

        }

        public JsonResult NonTeamMembers(int issueId)
        {
            List<MemberVM> memberList = new List<MemberVM>();
            /*
            var members = repo.GetNonIssueMembers(TeamID, issueId);
            foreach (var member in members)
            {
                var vm = new MemberVM {MemberType = member.JobTitle, Name = member.DisplayName, MemberID=member.ID };
                vm.AvatarHash = UserService.GetImageSource(member.EmailAddress,48);
                memberList.Add(vm);
            }*//*
            return Json ( new{ Data = memberList, Status = "Success" },JsonRequestBehavior.AllowGet);
        }
        */
        public ActionResult ActivityStream()
        {
            List<ActivityVM> list = new List<ActivityVM>();

            list= issueService.GetTeamActivityVMs(TeamID);


          //  var activityList = repo.GetTeamActivity(TeamID).Take(12);
            /*foreach (var activity in activityList)
            {
                var vm = new TeamActivityVM();
                vm.ItemName = activity.ItemName;
                vm.UserName = activity.CreatedBy.DisplayName;
                vm.UserID = activity.CreatedBy.ID;
                vm.Activity = activity.Action.ToLower();
                if (vm.Activity.ToUpper() == "CHANGED STATUS")
                {
                    vm.Activity += " of";

                    vm.NewState ="to "+ activity.NewState;
                }

                
                vm.EventDate = activity.CreatedDate.ToString("g");
                vm.EventDateRelative = activity.CreatedDate.ToRelativeDateTime();

                if (activity.ItemType == "Issue")
                {
                    vm.LinkURL = Url.Action("Details", "Issues", new { @id = activity.ItemID });
                }

                list.Add(vm);
            }*/
            if (Request.IsAjaxRequest())
                return PartialView("Partial/ActivityStream", list);

            return View(list);
        }
    }
}
