using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Routing;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Extensions;

namespace TeamBins6.Infrastrucutre.Services
{
    public interface ITeamManager
    {
        int SaveTeam(TeamDto team);
        TeamBins.Common.TeamDto GetTeam(int id);
        List<TeamDto> GetTeams();
        IEnumerable<ActivityDto> GeActivityItems(int count);
        bool DoesCurrentUserBelongsToTeam();

        Task<DashBoardItemSummaryVM> GetDashboardSummary();
        void Delete(int id);

        Task<TeamVM> GetTeamInoWithMembers();

        Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest);

        Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations();

    }
    public class TeamManager : ITeamManager
    {
        private IUserRepository userRepository;
        private IIssueRepository issueRepository;
        IActivityRepository activityRepository;
        IUserSessionHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        private IEmailManager emailManager;
        public TeamManager(IUserSessionHelper userSessionHelper, IActivityRepository activityRepository, ITeamRepository teamRepository, IIssueRepository issueRepository,IUserRepository userRepository, IEmailManager emailManager)
        {
            this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
            this.activityRepository = activityRepository;
            this.issueRepository = issueRepository;
            this.userRepository = userRepository;
            this.emailManager = emailManager;
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
                await teamRepository.SaveTeamMemberRequest(teamMemberRequest);
                await emailManager.SendTeamMemberInvitationEmail(teamMemberRequest);
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
                            DateInvited = s.CreatedDate.ToShortDateString()
                        }).ToList();


            vm.Members = members;

            return vm;
        }
        public TeamDto GetTeam(int id)
        {
            return this.teamRepository.GetTeam(id);
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

        public IEnumerable<ActivityDto> GeActivityItems(int count)
        {
            var activities = activityRepository.GetActivityItems(userSessionHelper.TeamId, count);

            foreach (var activity in activities)
            {
                if (activity.ObjectType == "Issue")
                {
                    activity.ObjectUrl = "Issue/" + activity.ObjectId;
                    if (activity.Description.ToUpper() == "CREATED")
                    {
                        activity.NewState = "";
                    }
                    else if (activity.Description.ToUpper() == "CHANGED STATUS")
                    {
                        activity.Description = "changed status of";

                        activity.NewState = "from " + activity.OldState + " to " + activity.NewState;
                    }
                    else if (activity.Description.ToUpper() == "DUE DATE UPDATED")
                    {
                        activity.Description = "updated due date of";
                        activity.NewState = "to " + activity.NewState;
                    }
                }
            }

            return activities;
            ;
        }

    }
}
