using System;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class UserAccountManager : IUserAccountManager
    {
        ITeamRepository teamRepository;
        readonly IAccountRepository accountRepository;
        IUserAccountEmailManager userAccountEmailManager;
        public UserAccountManager(IAccountRepository accountRepository, ITeamRepository teamRepository,IUserAccountEmailManager userAccountEmailManager)
        {
            this.accountRepository = accountRepository;
            this.teamRepository = teamRepository;
            this.userAccountEmailManager = userAccountEmailManager;
        }

        public bool DoesAccountExist(string email)
        {
            return accountRepository.DoesAccountExist(email);
        }
        public UserAccountDto GetUser(string email)
        {
            return accountRepository.GetUser(email);
        }

        public ResetPaswordRequestDto ProcessPasswordRecovery(string email)
        {
            var user = accountRepository.GetUser(email);
            if (user != null)
            {
                var uniqueLink = Guid.NewGuid().ToString("n") + user.Id;
                accountRepository.SavePasswordResetRequest(user, uniqueLink);

                userAccountEmailManager.SendResetPasswordEmail(user, uniqueLink);
                return new ResetPaswordRequestDto {};
            }
            return null;
        }

        public ResetPaswordRequestDto GetResetPaswordRequest(string id)
        {
            return accountRepository.GetResetPaswordRequest(id);
        }
        public async Task SaveLastLoginAsync(int userId)
        {
            await accountRepository.SaveLastLoginAsync(userId);
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

        public bool ResetPassword(string resetPasswordLink,string password)
        {
            var request = accountRepository.GetResetPaswordRequest(resetPasswordLink);
            if (request != null)
            {
                accountRepository.UpdatePassword(password,request.UserId);
                return true;
            }
            return false;
        }
    }
}