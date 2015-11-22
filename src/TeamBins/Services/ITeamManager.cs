using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.Services
{
    public interface ITeamManager
    {
        List<TeamDto> GetTeams();
        List<ActivityDto> GeActivityItems(int count);
    }
}