using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;

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
    }
}