using System;
using System.Linq;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TeamBins.Services;
using TechiesWeb.TeamBins.ViewModels;
namespace TechiesWeb.TeamBins.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController()
        {
            repo = new Repositary();
        }

        public UsersController(IRepositary repositary) : base(repositary)
        {
        }

        public ActionResult Index()
        {
            try
            {
                var team = repo.GetTeam(TeamID);
                var teamVM = new TeamVM { Name = team.Name, ID = team.ID };

                var teamMembers = team.TeamMembers.ToList();
                foreach (var member in teamMembers)
                {
                    var memberVM = new MemberVM();
                    memberVM.Name = member.Member.FirstName;
                    memberVM.EmailAddress = member.Member.EmailAddress;
                    memberVM.AvatarHash = UserService.GetImageSource(memberVM.EmailAddress, 30);
                    memberVM.JoinedDate = member.CreatedDate.ToShortDateString();
                    teamVM.Members.Add(memberVM);
                }
                return View(teamVM);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult Add()
        {
            return View(new AddTeamMemberRequestVM { TeamID = TeamID });
        }

        [HttpPost]
        public ActionResult Add(AddTeamMemberRequestVM model)
        {
            // This method adds a user to the team
            // If user is new to the system, sends an email to user to signup and join, else add him to the team
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = repo.GetUser(model.EmailAddress);
                    if (existingUser != null)
                    {
                        TeamMember teamMember = new TeamMember { MemberID = existingUser.ID, TeamID = TeamID, CreatedByID = UserID };
                        var result = repo.SaveTeamMember(teamMember);
                        if (result.ID > 0)
                        {                            
                            return Json(new { Status = "Success",Message="Successfully added user to team" });
                        }
                    }
                    else
                    {
                        var teamMemberRequest = new TeamMemberRequest { EmailAddress = model.EmailAddress, CreatedByID = UserID };
                        teamMemberRequest.TeamID = model.TeamID;
                        teamMemberRequest.ActivationCode = model.TeamID + "-" + Guid.NewGuid().ToString("n");
                        var resultNew = repo.SaveTeamMemberRequest(teamMemberRequest);
                        if (resultNew.Status)
                        {
                            new UserService(repo,SiteBaseURL).SendJoinMyTeamEmail(teamMemberRequest);
                            return Json(new { Status = "Success" });
                        }
                        else
                        {
                            log.Debug(resultNew);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);                
            }
            return Json(new { Status = "Error", Message = "Error adding user to team" });
        }

        public ActionResult JoinMyTeam(string id)
        {
            // For users who received an email with the join link to join a team.
            // The user must have created an account by now and coming back to this link after registration

            try
            {
                var teamMemberRequest = repo.GetTeamMemberRequest(id);
                if (teamMemberRequest != null)
                {
                    var user = repo.GetUser(teamMemberRequest.EmailAddress);
                    if (user.ID == UserID)
                    {
                        //Add to the team
                        var teamMember = new TeamMember { MemberID = UserID, TeamID = teamMemberRequest.TeamID, CreatedByID = teamMemberRequest.CreatedByID };
                        repo.SaveTeamMember(teamMember);
                       
                        //Keep that team as default team for the user 
                        SetUserIDToSession(UserID, teamMemberRequest.TeamID, user.FirstName);
                                                
                        var userService = new UserService(repo);
                        userService.SaveActivityForNewUserJoinedTeam(teamMemberRequest, user, UserID, TeamID);

                        //Correct user 
                        return View("WelcomeToTeam");
                    }
                }
                return View("NotFound");
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return View("Error");
            }
        }       
                
        public JsonResult TeamMembers(string term)
        {
            //Returns project member list in JSON format
            try
            {
                var team = repo.GetTeam(TeamID);

                var projectMembers = team.TeamMembers.Where(s => s.Member.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase)).Select(item => new { value = item.Member.FirstName, id = item.Member.ID.ToString() }).ToList();
                return Json(projectMembers, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return Json(new { Status = "Error", Message = "Troubles getting team members", JsonRequestBehavior.AllowGet });
            }
        }
    }
}
