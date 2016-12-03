using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

using TeamBins.DataAccessCore;
using TeamBins.Services;
using TeamBins.Infrastrucutre.Extensions;
using TeamBins.DataAccess;

namespace TeamBins.Infrastrucutre.Services
{
    public interface ITeamManager
    {
        Task<IEnumerable<ItemCount>> GetIssueCountPerProject(int teamId);
        Task<bool> ValidateAndAssociateNewUserToTeam(string activationCode);
        int SaveTeam(TeamDto team);
        TeamDto GetTeam(int id);
        TeamDto GetTeam(string name);
        List<TeamDto> GetTeams();
        IEnumerable<ActivityDto> GeActivityItems(int teamId, int count);
        bool DoesCurrentUserBelongsToTeam(int userId, int teamId);

        Task<DashBoardItemSummaryVm> GetDashboardSummary(int teamId);
        void Delete(int id);

        Task<TeamVM> GetTeamInoWithMembers();

        Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest);

        Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations();
        Task<IEnumerable<ChartItem>> GetIssueCountPerPriority(int teamId);

        Task SaveVisibility(int id, bool isPublic);

        Task EmailTest();
    }
    public class TeamManager : ITeamManager
    {
        private readonly IUserRepository userRepository;
        private readonly IIssueRepository issueRepository;
        readonly IActivityRepository activityRepository;
        readonly IUserAuthHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        private readonly IEmailManager emailManager;
        public TeamManager(IUserAuthHelper userSessionHelper, IActivityRepository activityRepository, ITeamRepository teamRepository, IIssueRepository issueRepository, IUserRepository userRepository, IEmailManager emailManager)
        {
            this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
            this.activityRepository = activityRepository;
            this.issueRepository = issueRepository;
            this.userRepository = userRepository;
            this.emailManager = emailManager;
        }

        public async Task EmailTest()
        {
            Email email =new Email();
            email.ToAddress.Add("kshyju@live.in");
            email.Subject = "Test at " + DateTime.Now;
            email.Body = "s";
            email.FromAddress = "connectshyju@gmail.com";
            this.emailManager.Send(email);
        }

        public async Task<bool> ValidateAndAssociateNewUserToTeam(string activationCode)
        {

            var invitation = await this.teamRepository.GetTeamMemberInvitation(activationCode);
            if (invitation != null)
            {
                var currentUser = await userRepository.GetUser(this.userSessionHelper.UserId);
                if (currentUser != null && currentUser.EmailAddress == invitation.EmailAddress)
                {
                    // Now asssociate this user to the team.
                    this.teamRepository.SaveTeamMember(invitation.TeamId, this.userSessionHelper.UserId, this.userSessionHelper.UserId);
                    this.userSessionHelper.SetTeamId(invitation.TeamId);

                    await this.teamRepository.DeleteTeamMemberInvitation(invitation.Id);
                    return true;
                }

            }
            return false;
        }

        public async Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations()
        {

            return await this.teamRepository.GetTeamMemberInvitations(this.userSessionHelper.TeamId);
        }

        public async Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest)
        {
            var user = await userRepository.GetUser(teamMemberRequest.EmailAddress);
            if (user != null)
            {
                teamRepository.SaveTeamMember(this.userSessionHelper.TeamId, user.Id, this.userSessionHelper.UserId);
            }
            else
            {
                teamMemberRequest.TeamId = this.userSessionHelper.TeamId;
                teamMemberRequest.CreatedById = this.userSessionHelper.UserId;
                var id = await teamRepository.SaveTeamMemberRequest(teamMemberRequest);
                var requests = await teamRepository.GetTeamMemberInvitations(this.userSessionHelper.TeamId)
                    ;
                var r = requests.FirstOrDefault(s => s.Id == id);

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
            var t = this.teamRepository.GetTeam(id);
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

        public bool DoesCurrentUserBelongsToTeam(int userId, int teamId)
        {
            var member = this.teamRepository.GetTeamMember(teamId, userId);
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
            team.CreatedById = this.userSessionHelper.UserId;
            var teamId = teamRepository.SaveTeam(team);
            return teamId;
        }

        public async Task<DashBoardItemSummaryVm> GetDashboardSummary(int teamId)
        {
            var teamIdtoGetDataFor = GetTeamIdtoGetDataFor(teamId);
            var vm = new DashBoardItemSummaryVm
            {
                IssueCountsByStatus = await issueRepository.GetIssueCountsPerStatus(teamIdtoGetDataFor)
            };
            return vm;
        }

        private int GetTeamIdtoGetDataFor(int teamId)
        {
            var teamIdtoGetDataFor = 0;
            var team = teamRepository.GetTeam(teamId);
            if (team != null)
            {
                if (team.IsPublic)
                {
                    teamIdtoGetDataFor = team.Id;
                }
                else
                {
                    //May be a valid user requested for his private team
                    if (team.Id == this.userSessionHelper.TeamId)
                    {
                        teamIdtoGetDataFor = team.Id;
                    }
                }
            }
            return teamIdtoGetDataFor;
        }

        public async Task<IEnumerable<ChartItem>> GetIssueCountPerPriority(int teamId)
        {
            var teamIdtoGetDataFor = GetTeamIdtoGetDataFor(teamId);

            var issueCountsByStatus = await issueRepository.GetIssueCountsPerPriority(teamIdtoGetDataFor);
            return issueCountsByStatus;
        }

        public async Task<IEnumerable<ItemCount>> GetIssueCountPerProject(int teamId)
        {
            var teamIdtoGetDataFor = GetTeamIdtoGetDataFor(teamId);

            var issueCountsByProject = await issueRepository.GetIssueCountsPerProject(teamIdtoGetDataFor);
            var totalIssueCount = issueCountsByProject.Sum(g => g.Count);
            foreach (var project in issueCountsByProject)
            {
                var s = ((decimal) project.Count/totalIssueCount)*100;
                project.Percentage = Convert.ToInt32(s);
            }
            return issueCountsByProject;
        }


        public IEnumerable<ActivityDto> GeActivityItems(int teamId, int count)
        {
            var activities = activityRepository.GetActivityItems(teamId, count);

            foreach (var activity in activities)
            {
                if (activity.ObjectType == "Issue")
                {
                    activity.ObjectUrl = "Issues/" + activity.ObjectId;
                    if (String.Equals(activity.Description, "CREATED", StringComparison.OrdinalIgnoreCase))
                    {
                        activity.NewState = "";
                    }
                    else if (String.Equals(activity.Description, "CHANGED STATUS", StringComparison.OrdinalIgnoreCase))
                    {
                        activity.Description = "changed status of";

                        activity.NewState = "from " + activity.OldState + " to " + activity.NewState;
                    }
                    else if (String.Equals(activity.Description, "Changed category", StringComparison.OrdinalIgnoreCase))
                    {
                        activity.Description = "changed category of";

                        activity.NewState = "from " + activity.OldState + " to " + activity.NewState;
                    }
                    else if (String.Equals(activity.Description, "DUE DATE UPDATED", StringComparison.OrdinalIgnoreCase))
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

        }

        public async Task SaveVisibility(int id, bool isPublic)
        {
            await this.teamRepository.SaveVisibility(id, isPublic);
        }
    }
}
