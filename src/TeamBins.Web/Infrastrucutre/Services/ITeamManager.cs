using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        
    }
    public class TeamManager : ITeamManager
    {
        readonly AppSettings _settings;

        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIssueRepository _issueRepository;
        readonly IActivityRepository _activityRepository;
        readonly IUserAuthHelper _userSessionHelper;
        private readonly ITeamRepository _teamRepository;
        private readonly IEmailManager _emailManager;
        public TeamManager(IUserAuthHelper userSessionHelper, IActivityRepository activityRepository,
            ITeamRepository teamRepository, IIssueRepository issueRepository, IUserRepository userRepository, 
            IEmailManager emailManager,IEmailRepository emailRepository, IOptions<AppSettings> settings)
        {
            this._emailRepository = emailRepository;
            this._teamRepository = teamRepository;
            this._userSessionHelper = userSessionHelper;
            this._activityRepository = activityRepository;
            this._issueRepository = issueRepository;
            this._userRepository = userRepository;
            this._emailManager = emailManager;
            this._settings = settings.Value;
        }
        
        public async Task<bool> ValidateAndAssociateNewUserToTeam(string activationCode)
        {

            var invitation = await this._teamRepository.GetTeamMemberInvitation(activationCode);
            if (invitation != null)
            {
                var currentUser = await _userRepository.GetUser(this._userSessionHelper.UserId);
                if (currentUser != null && currentUser.EmailAddress == invitation.EmailAddress)
                {
                    // Now asssociate this user to the team.
                    this._teamRepository.SaveTeamMember(invitation.TeamId, this._userSessionHelper.UserId, this._userSessionHelper.UserId);
                    this._userSessionHelper.SetTeamId(invitation.TeamId);

                    await this._teamRepository.DeleteTeamMemberInvitation(invitation.Id);
                    return true;
                }

            }
            return false;
        }

        public async Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations()
        {

            return await this._teamRepository.GetTeamMemberInvitations(this._userSessionHelper.TeamId);
        }

        private async Task SendTeamMemberInvitationEmail(AddTeamMemberRequestVM teamMemberRequest)
        {
            try
            {
                var emailTemplate = await _emailRepository.GetEmailTemplate("JoinMyTeam");
                if (emailTemplate != null)
                {
                    var emailSubject = emailTemplate.Subject;
                    var emailBody = emailTemplate.EmailBody;
                    var email = new Email();
                    email.ToAddress.Add(teamMemberRequest.EmailAddress);

                    var joinLink = String.Format("{0}Account/Join?returnurl={1}", this._settings.SiteUrl, teamMemberRequest.ActivationCode);
                    emailBody = emailBody.Replace("@teamName", teamMemberRequest.Team.Name);
                    emailBody = emailBody.Replace("@joinUrl", joinLink);
                    emailBody = emailBody.Replace("@inviter", teamMemberRequest.CreatedBy.Name);
                    email.Body = emailBody;
                    email.Subject = emailSubject;
                    await this._emailManager.Send(email);
                }
            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }

        }

        public async Task AddNewTeamMember(AddTeamMemberRequestVM teamMemberRequest)
        {
            var user = await _userRepository.GetUser(teamMemberRequest.EmailAddress);
            if (user != null)
            {
                _teamRepository.SaveTeamMember(this._userSessionHelper.TeamId, user.Id, this._userSessionHelper.UserId);
            }
            else
            {
                teamMemberRequest.TeamId = this._userSessionHelper.TeamId;
                teamMemberRequest.CreatedById = this._userSessionHelper.UserId;
                var id = await _teamRepository.SaveTeamMemberRequest(teamMemberRequest);
                var requests = await _teamRepository.GetTeamMemberInvitations(this._userSessionHelper.TeamId)
                    ;
                var r = requests.FirstOrDefault(s => s.Id == id);

                await SendTeamMemberInvitationEmail(r);
            }
        }

        public async Task<TeamVM> GetTeamInoWithMembers()
        {
            var vm = new TeamVM();

            var team = _teamRepository.GetTeam(this._userSessionHelper.TeamId);
            vm.Name = team.Name;

            var members = await _teamRepository.GetTeamMembers(this._userSessionHelper.TeamId);
            foreach (var teamMemberDto in members)
            {
                teamMemberDto.GravatarUrl = teamMemberDto.EmailAddress.ToGravatarUrl();
            }

            var invitations = await _teamRepository.GetTeamMemberInvitations(this._userSessionHelper.TeamId);
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
            var t = this._teamRepository.GetTeam(id);
            if (t != null)
            {
                t.IsRequestingUserTeamOwner = t.CreatedById == _userSessionHelper.UserId;
                return t;
            }
            return null;
        }
        public TeamDto GetTeam(string name)
        {
            return this._teamRepository.GetTeam(name);
        }

        public void Delete(int id)
        {
            _teamRepository.Delete(id);
        }

        public bool DoesCurrentUserBelongsToTeam(int userId, int teamId)
        {
            var member = this._teamRepository.GetTeamMember(teamId, userId);
            return member != null;

        }
        public List<TeamDto> GetTeams()
        {
            var teams = _teamRepository.GetTeams(_userSessionHelper.UserId);
            foreach (var teamDto in teams)
            {
                teamDto.IsRequestingUserTeamOwner = teamDto.CreatedById == this._userSessionHelper.UserId;
            }
            return teams;
        }

        public int SaveTeam(TeamDto team)
        {
            team.CreatedById = this._userSessionHelper.UserId;
            var teamId = _teamRepository.SaveTeam(team);
            return teamId;
        }

        public async Task<DashBoardItemSummaryVm> GetDashboardSummary(int teamId)
        {
            var teamIdtoGetDataFor = GetTeamIdtoGetDataFor(teamId);
            var vm = new DashBoardItemSummaryVm
            {
                IssueCountsByStatus = await _issueRepository.GetIssueCountsPerStatus(teamIdtoGetDataFor)
            };
            return vm;
        }

        private int GetTeamIdtoGetDataFor(int teamId)
        {
            var teamIdtoGetDataFor = 0;
            var team = _teamRepository.GetTeam(teamId);
            if (team != null)
            {
                if (team.IsPublic)
                {
                    teamIdtoGetDataFor = team.Id;
                }
                else
                {
                    //May be a valid user requested for his private team
                    if (team.Id == this._userSessionHelper.TeamId)
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

            var issueCountsByStatus = await _issueRepository.GetIssueCountsPerPriority(teamIdtoGetDataFor);
            return issueCountsByStatus;
        }

        public async Task<IEnumerable<ItemCount>> GetIssueCountPerProject(int teamId)
        {
            var teamIdtoGetDataFor = GetTeamIdtoGetDataFor(teamId);

            var issueCountsByProject = await _issueRepository.GetIssueCountsPerProject(teamIdtoGetDataFor);
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
            var activities = _activityRepository.GetActivityItems(teamId, count);

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
                    if (activity.Description.Equals("Commented", StringComparison.OrdinalIgnoreCase))
                    {
                        activity.Description = "Commented on ";
                    }
                }
            }

            return activities;

        }

        public async Task SaveVisibility(int id, bool isPublic)
        {
            await this._teamRepository.SaveVisibility(id, isPublic);
        }
    }
}
