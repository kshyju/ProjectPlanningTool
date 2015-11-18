using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class TeamManager : ITeamManager
    {
        IUserSessionHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        public TeamManager(ITeamRepository teamRepository,IUserSessionHelper userSessionHelper)
        {
            this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
        }

        public List<TeamDto> GetTeams()
        {
            return teamRepository.GetTeams(userSessionHelper.UserId);
        } 


    }
}