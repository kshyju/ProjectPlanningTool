using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class UserAccountManager : IUserAccountManager
    {
        ITeamRepository teamRepository;
        readonly IAccountRepository accountRepository;
        public UserAccountManager(IAccountRepository accountRepository, ITeamRepository teamRepository)
        {
            this.accountRepository = accountRepository;
            this.teamRepository = teamRepository;
        }

        public bool DoesAccountExist(string email)
        {
            return accountRepository.DoesAccountExist(email);
        }
        public UserAccountDto GetUser(string email)
        {
            return accountRepository.GetUser(email);
        }

        public LoggedInSessionInfo CreateUserAccount(UserAccountDto userAccount)
        {
            var userSession = new LoggedInSessionInfo {};
            userAccount.GravatarUrl = UserService.GetGravatarHash(userAccount.EmailAddress);
            var userId = accountRepository.Save(userAccount);

            //Create a default team for the user
            var team = new TeamDto { Name = userAccount.Name.Replace(" ", "-"), CreatedById = userId };
            if (team.Name.Length > 19)
                team.Name = team.Name.Substring(0, 19);

            var teamId = teamRepository.SaveTeam(team);
            teamRepository.SaveTeamMember(teamId,userId,userId);

            userSession.TeamId = teamId;
            userSession.UserId = userId;
            userSession.UserDisplayName = userAccount.Name;
            return userSession;
        }
    }
}