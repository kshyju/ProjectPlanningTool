using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Glimpse.AspNet.Tab;
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
        IUserSessionHelper userSessionHelper;
        IProjectRepository projectRepository;
        public UserAccountManager(IAccountRepository accountRepository, ITeamRepository teamRepository, IUserAccountEmailManager userAccountEmailManager, IUserSessionHelper userSessionHelper, IProjectRepository projectRepository)
        {
            this.accountRepository = accountRepository;
            this.teamRepository = teamRepository;
            this.userAccountEmailManager = userAccountEmailManager;
            this.userSessionHelper = userSessionHelper;
            this.projectRepository = projectRepository;
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
                return new ResetPaswordRequestDto { };
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
            var userSession = new LoggedInSessionInfo { };
            userAccount.GravatarUrl = UserService.GetGravatarHash(userAccount.EmailAddress);
            var userId = accountRepository.Save(userAccount);

            //Create a default team for the user
            var team = new TeamDto { Name = userAccount.Name.Replace(" ", "-"), CreatedById = userId };
            if (team.Name.Length > 19)
                team.Name = team.Name.Substring(0, 19);

            var teamId = teamRepository.SaveTeam(team);
            teamRepository.SaveTeamMember(teamId, userId, userId);

            teamRepository.SaveDefaultTeamForUser(userId,teamId);

            userSession.TeamId = teamId;
            userSession.UserId = userId;
            userSession.UserDisplayName = userAccount.Name;
            return userSession;
        }

        public bool ResetPassword(string resetPasswordLink, string password)
        {
            var request = accountRepository.GetResetPaswordRequest(resetPasswordLink);
            if (request == null) return false;
            accountRepository.UpdatePassword(password, request.UserId);
            return true;
        }

        public void UpdateProfile(EditProfileVm model)
        {
            accountRepository.UpdateProfile(new UserAccountDto {Name = model.Name, EmailAddress = model.Email});
        }

        public void UpdatePassword(ChangePasswordVM model)
        {
            accountRepository.UpdatePassword(model.Password, userSessionHelper.UserId);
        }

        public EditProfileVm GetUserProfile()
        {
            var user = accountRepository.GetUser(userSessionHelper.UserId);
            if (user == null) return null;
            return new EditProfileVm { Name = user.Name, Email = user.EmailAddress };
        }

        public UserEmailNotificationSettingsVM GetNotificationSettings()
        {
            return  accountRepository.GetUserNotificationSettings(userSessionHelper.UserId,userSessionHelper.TeamId);
        }

        public DefaultIssueSettings GetIssueSettingsForUser()
        {
            var vm = new DefaultIssueSettings();
            vm.Projects = projectRepository.GetProjects(userSessionHelper.TeamId).Select(s=> new SelectListItem { Value = s.Id.ToString(), Text = s.Name}).ToList();
            vm.SelectedProject =  accountRepository.GetDefaultProjectForIssues(userSessionHelper.UserId, userSessionHelper.TeamId);

            return vm;
        }

        public void SaveDefaultProjectForTeam(int? selectedProject)
        {
            teamRepository.SaveDefaultProject(userSessionHelper.UserId, userSessionHelper.TeamId, selectedProject);
        }
    }
}