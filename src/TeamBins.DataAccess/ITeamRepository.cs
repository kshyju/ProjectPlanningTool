using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface ITeamRepository
    {
        TeamDto GetTeam(int teamId);
        int SaveTeam(TeamDto team);

        void SaveTeamMember(int teamId, int memberId, int createdById);
    }
}