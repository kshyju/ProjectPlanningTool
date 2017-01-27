using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface ITeamRepository
    {
        IEnumerable<TeamDto> GetTeams();
        TeamDto GetTeam(int teamId);
        TeamDto GetTeam(string name);
        int SaveTeam(TeamDto team);

        void SaveTeamMember(int teamId, int memberId, int createdById);
        void SaveDefaultProject(int userId, int teamId, int? selectedProject);
        List<TeamDto> GetTeams(int userId);

        void SaveDefaultTeamForUser(int userId, int teamId);
        MemberVM GetTeamMember(int teamId, int userId);
        void Delete(int id);
        Task<IEnumerable<TeamMemberDto>> GetTeamMembers(int teamId);

        Task<int> SaveTeamMemberRequest(AddTeamMemberRequestVM teamMemberRequest);
        Task<IEnumerable<AddTeamMemberRequestVM>> GetTeamMemberInvitations(int teamId);

        Task<AddTeamMemberRequestVM> GetTeamMemberInvitation(string activationCode);
        Task DeleteTeamMemberInvitation(int id);
        Task SaveVisibility(int id, bool isPublic);

        /// <summary>
        /// Get the list of email subscribers
        /// </summary>
        /// <param name="teamId">The teamId</param>
        /// <param name="notificationtypeCode">The notification type code (Ex : NewIssue) </param>
        /// <returns></returns>
        Task<IEnumerable<UserDto>> GetSubscribers(int teamId, string notificationtypeCode);
    }
}