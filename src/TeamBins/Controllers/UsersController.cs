using Planner.DataAccess;
using Planner.Services;
using SmartPlan.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechiesWeb.TeamBins.ViewModels;

namespace Planner.Controllers
{
    public class UsersController : BaseController
    {
        protected IRepositary repo;
       
        public UsersController()
        {
            repo = new Repositary();

        }
        public ActionResult Index()
        {
            
            var team=repo.GetTeam(TeamID);
            var teamVM = new TeamVM { Name = team.Name, ID = team.ID };

            var teamMembers = team.TeamMembers.ToList();
            foreach (var member in teamMembers)
            {
                var memberVM = new MemberVM();
                memberVM.Name=member.Member.FirstName;
                memberVM.EmailAddress = member.Member.EmailAddress;
                memberVM.AvatarHash = UserService.GetImageSource(memberVM.EmailAddress, 30);
                memberVM.JoinedDate = member.CreatedDate.ToShortDateString();
                teamVM.Members.Add(memberVM);
            }
            return View(teamVM);
        }

        public ActionResult Add()
        {
            return View(new AddTeamMemberRequestVM { TeamID = TeamID });
        }

        [HttpPost]
        public ActionResult Add(AddTeamMemberRequestVM model)
        {
            // We are now allowing only existing members to be added to a team. 
            // TO DO : Save the add member to team request in a table and email the new user with a link to login & join

            if (ModelState.IsValid)
            {
                var existingUser = repo.GetUser(model.EmailAddress);
                if (existingUser != null)
                {
                    TeamMember teamMember = new TeamMember { MemberID = existingUser.ID, TeamID = TeamID, CreatedByID = UserID };
                    var result = repo.SaveTeamMember(teamMember);
                    if (result.ID > 0)
                    {
                        //Save to Activity stream
                       // var activityStream=new Activity { ObjectType="TeamMember",Activity1="Added", CreatedByID=UserID, NewState= 

                        return Json(new { Status = "Success" });
                    }
                }
                else
                {
                    // to do : Save to Req table and then send email to this member with a unique link
                }
            }           
            return View(model);
        }

        public JsonResult TeamMembers(string term)
        {
            //Returns project member list in JSON format
            var team = repo.GetTeam(TeamID);

            var projectMembers = team.TeamMembers.Where(s => s.Member.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase)).Select(item => new { value = item.Member.FirstName, id = item.Member.ID.ToString() }).ToList();
            return Json(projectMembers, JsonRequestBehavior.AllowGet);
        }
    }
}
