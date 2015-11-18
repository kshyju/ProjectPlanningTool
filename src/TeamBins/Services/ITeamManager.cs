using System.Collections.Generic;
using TeamBins.Common;

namespace TeamBins.Services
{
    public interface ITeamManager
    {
        List<TeamDto> GetTeams();
    }
}