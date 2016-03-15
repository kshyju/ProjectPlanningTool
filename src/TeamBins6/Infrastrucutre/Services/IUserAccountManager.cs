using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;
using System.Collections.Generic;

namespace TeamBins.Services
{

    public interface IUserAccountManager
    {
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

        //void UpdateProfile(EditProfileVm model);
        //void UpdatePassword(ChangePasswordVM model);
        //void SaveDefaultProjectForTeam(int? selectedProject);
    }

    public class UserAccountManager : IUserAccountManager
    {
        private IUserRepository userRepository;
        public UserAccountManager(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public  async Task SetDefaultTeam(int userId, int teamId)
        {
            await this.userRepository.SetDefaultTeam(userId, teamId);
        }
        public async Task<UserAccountDto> GetUser(int id)
        {
            return await this.userRepository.GetUser(id);
        }

        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
            return await userRepository.GetTeams(userId);

        }
    }

}