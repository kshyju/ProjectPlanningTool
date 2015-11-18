using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface IAccountRepository
    {
        int Save(UserAccountDto userAccount);
        bool DoesAccountExist(string email);

        UserAccountDto GetUser(string email);
        UserAccountDto GetUser(int userId);

        Task SaveLastLoginAsync(int userId);

        void SavePasswordResetRequest(UserAccountDto userAccount, string activationLink);

        ResetPaswordRequestDto GetResetPaswordRequest(string id);

        void UpdatePassword(string password, int userId);
        UserEmailNotificationSettingsVM GetUserNotificationSettings(int userId, int teamId);
        int? GetDefaultProjectForIssues(int userId, int teamId);

        void UpdateProfile(UserAccountDto userAccountDto);
    }
}