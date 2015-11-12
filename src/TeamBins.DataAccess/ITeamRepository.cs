using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface ITeamRepository
    {
        TeamDto GetTeam(int teamId);
        void SaveTeam(TeamDto team);
    }
}