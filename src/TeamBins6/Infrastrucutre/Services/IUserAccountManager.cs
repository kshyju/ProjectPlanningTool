using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;
using System.Collections.Generic;
using System.Linq;

namespace TeamBins.Services
{

    public interface IUserAccountManager
    {
        Task<DefaultIssueSettings> GetIssueSettingsForUser();
        Task<EditProfileVm> GetUserProfile();
        Task SetDefaultTeam(int userId, int teamId);
        Task<IEnumerable<TeamDto>> GetTeams(int userId);
        Task<UserAccountDto> GetUser(int id);
        //bool DoesAccountExist(string email);
        //LoggedInSessionInfo CreateUserAccount(UserAccountDto userAccount);

        //UserAccountDto GetUser(string email);
        //UserAccountDto GetUser(int id);

        //Task SaveLastLoginAsync(int userId);

        //ResetPaswordRequestDto ProcessPasswordRecovery(string email);

        //ResetPaswordRequestDto GetResetPaswordRequest(string id);

        //bool ResetPassword(string resetPasswordLink, string password);

        //// ResetPasswordVM 
        //EditProfileVm GetUserProfile();
        //UserEmailNotificationSettingsVM GetNotificationSettings();
        //DefaultIssueSettings GetIssueSettingsForUser();

        Task UpdateProfile(EditProfileVm model);
        //void UpdatePassword(ChangePasswordVM model);
        Task SaveDefaultProjectForTeam(DefaultIssueSettings defaultIssueSettings);
        Task<UserAccountDto> GetUser(string email);
        Task<LoggedInSessionInfo> CreateAccount(UserAccountDto userAccount);
    }

    public class UserAccountManager : IUserAccountManager
    {
        private IProjectManager projectManager;
        private IUserRepository userRepository;
        private IUserSessionHelper userSessionHelper;
        private ITeamRepository teamRepository;
        public UserAccountManager(IUserRepository userRepository, IUserSessionHelper userSessionHelper, IProjectManager projectManager,ITeamRepository teamRepository)
        {
            this.userRepository = userRepository;
            this.userSessionHelper = userSessionHelper;
            this.projectManager = projectManager;
            this.teamRepository = teamRepository;
        }

        public async Task SaveDefaultProjectForTeam(DefaultIssueSettings defaultIssueSettings)
        {
             await this.userRepository.SaveDefaultIssueSettings(defaultIssueSettings);
        }
        public async Task<EditProfileVm> GetUserProfile()
        {
            var vm = new EditProfileVm();
            var user = await this.userRepository.GetUser(this.userSessionHelper.UserId);
            if (user != null)
            {
                vm.Name = user.Name;
                vm.Email = user.EmailAddress;
            }
            return vm;
        }

      

        public async Task<DefaultIssueSettings> GetIssueSettingsForUser()
        {
            var vm = new DefaultIssueSettings();
            vm.Projects=this.projectManager.GetProjects()
                    .Select(s => new SelectListItem {Value = s.Id.ToString(), Text = s.Name})
                    .ToList();
            
            return await Task.FromResult(vm);
        }


        public  async Task SetDefaultTeam(int userId, int teamId)
        {
            await this.userRepository.SetDefaultTeam(userId, teamId);
        }
        public async Task<UserAccountDto> GetUser(int id)
        {
            return await this.userRepository.GetUser(id);
        }
        public async Task<UserAccountDto> GetUser(string email)
        {
            return await this.userRepository.GetUser(email);
        }
        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
            return await userRepository.GetTeams(userId);

        }

        public async Task<LoggedInSessionInfo> CreateAccount(UserAccountDto userAccount)
        {
            var userId = await userRepository.CreateAccount(userAccount);
            var teamId = await Task.FromResult(teamRepository.SaveTeam(new TeamDto {CreatedById = userId, Name = userAccount.Name + "'s Team"}));
            return new LoggedInSessionInfo() {TeamId = teamId, UserId = userId, UserDisplayName = userAccount.Name};
        }
        public async Task UpdateProfile(EditProfileVm model)
        {
            model.Id = this.userSessionHelper.UserId;
            await userRepository.SaveUserProfile(model);
        }
    }

}