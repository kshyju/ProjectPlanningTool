using System.Collections.Generic;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface ITeamRepository
    {
        TeamDto GetTeam(int teamId);
        int SaveTeam(TeamDto team);

        void SaveTeamMember(int teamId, int memberId, int createdById);
        void SaveDefaultProject(int userId, int teamId, int? selectedProject);
        List<TeamDto> GetTeams(int userId);

        void SaveDefaultTeamForUser(int userId, int teamId);
        MemberVM GetTeamMember(int teamId, int userId);
    }
}