using SmartPlan.ViewModels;
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

        [VerifyLogin]
        public ActionResult Index()
        {
            try
            {
                var team = repo.GetTeam(TeamID);
                var teamVM = new TeamVM { Name = team.Name, ID = team.ID };

                var teamMembers = team.TeamMembers.OrderBy(s=>s.Member.FirstName).ToList();
                foreach (var member in teamMembers)
                {
                    var memberVM = new MemberVM();
                    memberVM.Name = member.Member.FirstName;
                    memberVM.EmailAddress = member.Member.EmailAddress;
                    memberVM.AvatarHash = UserService.GetAvatarUrl(member.Member.Avatar, 30);
                    memberVM.JoinedDate = member.CreatedDate.ToShortDateString();
                    if(member.Member.LastLoginDate.HasValue)
                        memberVM.LastLoginDate=member.Member.LastLoginDate.Value.ToString("g");

                    teamVM.Members.Add(memberVM);
                }

                var membersNotJoinedList = repo.GetTeamMembersWhoHasntJoined(TeamID).OrderBy(s=>s.EmailAddress).ToList();
                foreach (var member in membersNotJoinedList)
                {
                    var invitation = new MemberInvitation { EmailAddress = member.EmailAddress, DateInvited = member.CreatedDate.ToString("g") };
                    invitation.AvatarHash = UserService.GetImageSource(member.EmailAddress);
                    teamVM.MembersInvited.Add(invitation);
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
        [VerifyLogin]
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

        [ChildActionOnly]
        public ActionResult MenuHeader()
        {
            var vm = new UserMenuHeaderVM();
            try
            {
                var user = repo.GetUser(UserID);
                if (user != null)
                {
                    vm.UserDisplayName = user.FirstName;
                    vm.UserAvatarHash = UserService.GetAvatarUrl(user.Avatar);
                    var teams = repo.GetTeams(UserID).ToList();
                    foreach (var team in teams)
                    {
                        var teamVM = new TeamVM { ID = team.ID, Name = team.Name };
                        vm.Teams.Add(teamVM);
                        if (team.ID == TeamID)
                            vm.CurrentTeamName = team.Name;
                    }
                    vm.SelectedTeam = TeamID;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return PartialView("Partial/MenuHeader",vm);
        }


        [VerifyLogin]
        public JsonResult TeamMembers(string term,int issueId)
        {
            //Returns team members who are not assigned to the issue in a list in JSON format
            try
            {
                var team = repo.GetTeam(TeamID);
               
                var projectMembers = team.TeamMembers
                    .Where(t=>!repo.GetIssueMembers(issueId).Any(s=>s.MemberID==t.MemberID))
                                            .Where(s => s.Member.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase))                                            
                                            .Select(item => new { value = item.Member.FirstName, id = item.Member.ID.ToString() }).ToList();
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
