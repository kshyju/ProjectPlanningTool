using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.Services
{
    public interface IUserAccountManager
    {
        bool DoesAccountExist(string email);
        LoggedInSessionInfo CreateUserAccount(UserAccountDto userAccount);

        UserAccountDto GetUser(string email);

        Task SaveLastLoginAsync(int userId);

        ResetPaswordRequestDto ProcessPasswordRecovery(string email);

        ResetPaswordRequestDto GetResetPaswordRequest(string id);

        bool ResetPassword(string resetPasswordLink,string password);

        // ResetPasswordVM 
        EditProfileVm GetUserProfile();
        UserEmailNotificationSettingsVM GetNotificationSettings();
        DefaultIssueSettings GetIssueSettingsForUser();

        void UpdateProfile(EditProfileVm model);
        void UpdatePassword(ChangePasswordVM model);
        void SaveDefaultProjectForTeam(int? selectedProject);
    }
}