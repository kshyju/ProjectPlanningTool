using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

using TeamBins.DataAccessCore;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Extensions;
using TeamBinsCore.DataAccess;

namespace TeamBins6.Infrastrucutre.Services
{
    public interface ITeamManager
    {
        Task<bool> ValidateAndAssociateNewUserToTeam(string activationCode);
        int SaveTeam(TeamDto team);
        TeamDto GetTeam(int id);
        List<TeamDto> GetTeams();
        IEnumerable<ActivityDto> GeActivityItems(int teamId, int count);
        bool DoesCurrentUserBelongsToTeam();

        Task<DashBoardItemSummaryVM> GetDashboardSummary();
        void Delete(int id);

        Task<TeamVM> GetTeamInoWithMembers();

        Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest);

        Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations();


        Task SaveVisibility(int id, bool isPublic);
    }
    public class TeamManager : ITeamManager
    {
        private IUserRepository userRepository;
        private IIssueRepository issueRepository;
        IActivityRepository activityRepository;
        IUserAuthHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        private IEmailManager emailManager;
        public TeamManager(IUserAuthHelper userSessionHelper, IActivityRepository activityRepository, ITeamRepository teamRepository, IIssueRepository issueRepository,IUserRepository userRepository, IEmailManager emailManager)
        {
            this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
            this.activityRepository = activityRepository;
            this.issueRepository = issueRepository;
            this.userRepository = userRepository;
            this.emailManager = emailManager;
        }

        public async Task<bool> ValidateAndAssociateNewUserToTeam(string activationCode)
        {

            var invitation =  await this.teamRepository.GetTeamMemberInvitation(activationCode);
            if(invitation!=null)
            {
                var currentUser = await userRepository.GetUser(this.userSessionHelper.UserId);
                if(currentUser!=null && currentUser.EmailAddress ==invitation.EmailAddress)
                {
                    // Now asssociate this user to the team.
                    this.teamRepository.SaveTeamMember(invitation.TeamID, this.userSessionHelper.UserId,this.userSessionHelper.UserId);
                    this.userSessionHelper.SetTeamId(invitation.TeamID);

                    await this.teamRepository.DeleteTeamMemberInvitation(invitation.Id);
                    return true;
                }

            }
            return false;
        }

        public async Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations()
        {
             
           return  await this.teamRepository.GetTeamMemberInvitations(this.userSessionHelper.TeamId);
        }

        public async  Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest)
        {
            var user = await userRepository.GetUser(teamMemberRequest.EmailAddress);
            if (user != null)
            {
                teamRepository.SaveTeamMember(this.userSessionHelper.TeamId,user.Id, this.userSessionHelper.UserId);                
            }
            else
            {
                teamMemberRequest.TeamID = this.userSessionHelper.TeamId;
                teamMemberRequest.CreatedById = this.userSessionHelper.UserId;
                var id=await teamRepository.SaveTeamMemberRequest(teamMemberRequest);
                var requests = await teamRepository.GetTeamMemberInvitations(this.userSessionHelper.TeamId)
                    ;
                var r= requests.FirstOrDefault(s => s.Id == id);
                
                await emailManager.SendTeamMemberInvitationEmail(r);
            }
        }

        public async Task<TeamVM> GetTeamInoWithMembers()
        {
            var vm = new TeamVM();

            var team = teamRepository.GetTeam(this.userSessionHelper.TeamId);
            vm.Name = team.Name;

            var members = await teamRepository.GetTeamMembers(this.userSessionHelper.TeamId);
            foreach (var teamMemberDto in members)
            {
                teamMemberDto.GravatarUrl = teamMemberDto.EmailAddress.ToGravatarUrl();
            }

            var invitations = await teamRepository.GetTeamMemberInvitations(this.userSessionHelper.TeamId);
            foreach (var teamMemberDto in invitations)
            {
                teamMemberDto.GravatarUrl = teamMemberDto.EmailAddress.ToGravatarUrl();
            }
            vm.MembersInvited =
                invitations.Select(
                    s =>
                        new MemberInvitation
                        {
                            EmailAddress = s.EmailAddress,
                            AvatarHash = s.GravatarUrl,
                            DateInvited = s.CreatedDate.ToString()
                        }).ToList();


            vm.Members = members;

            return vm;
        }
        public TeamDto GetTeam(int id)
        {
            var t= this.teamRepository.GetTeam(id);
            if (t != null)
            {
                t.IsRequestingUserTeamOwner = t.CreatedById == userSessionHelper.UserId;
                return t;
            }
            return null;
        }
        public TeamDto GetTeam(string name)
        {
            return this.teamRepository.GetTeam(name);
        }

        public void Delete(int id)
        {
            teamRepository.Delete(id);
        }

        public bool DoesCurrentUserBelongsToTeam()
        {
            var member = this.teamRepository.GetTeamMember(this.userSessionHelper.TeamId, this.userSessionHelper.UserId);
            return member != null;

        }
        public List<TeamDto> GetTeams()
        {
            var teams = teamRepository.GetTeams(userSessionHelper.UserId);
            foreach (var teamDto in teams)
            {
                teamDto.IsRequestingUserTeamOwner = teamDto.CreatedById == this.userSessionHelper.UserId;
            }
            return teams;
        }

        public int SaveTeam(TeamDto team)
        {
            var isNewTeam = team.Id == 0;
            team.CreatedById = this.userSessionHelper.UserId;
            var teamId = teamRepository.SaveTeam(team);
            return teamId;
        }

        public async Task<DashBoardItemSummaryVM> GetDashboardSummary()
        {
            var vm = new DashBoardItemSummaryVM
            {
                IssueCountsByStatus = await issueRepository.GetIssueCountsPerStatus(this.userSessionHelper.TeamId)
            };
            return vm;
        }

        public IEnumerable<ActivityDto> GeActivityItems(int teamId,int count)
        {
            var activities = activityRepository.GetActivityItems(teamId, count);

            foreach (var activity in activities)
            {
                if (activity.ObjectType == "Issue")
                {
                    activity.ObjectUrl = "Issues/" + activity.ObjectId;
                    if (String.Equals(activity.Description , "CREATED",StringComparison.OrdinalIgnoreCase))
                    {
                        activity.NewState = "";
                    }
                    else if (String.Equals(activity.Description, "CHANGED STATUS", StringComparison.OrdinalIgnoreCase))
                    {
                        activity.Description = "changed status of";

                        activity.NewState = "from " + activity.OldState + " to " + activity.NewState;
                    }
                    else if (String.Equals(activity.Description, "DUE DATE UPDATED",StringComparison.OrdinalIgnoreCase))
                    {
                        activity.Description = "updated due date of";
                        activity.NewState = "to " + activity.NewState;
                    }
                }
                else if (activity.ObjectType == "Comment")
                {
                    activity.ObjectUrl = "Issues/" + activity.ObjectId;
                    if (activity.Description.ToUpper() == "COMMENTED")
                    {
                        activity.NewState = "";
                    }
                }
            }

            return activities;
            ;
        }

        public async Task SaveVisibility(int id, bool isPublic)
        {
            await this.teamRepository.SaveVisibility(id, isPublic);
        }
    }
}
