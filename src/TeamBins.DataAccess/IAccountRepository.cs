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


        Task SaveLastLoginAsync(int userId);

        void SavePasswordResetRequest(UserAccountDto userAccount, string activationLink);

        ResetPaswordRequestDto GetResetPaswordRequest(string id);

        void UpdatePassword(string password, int userId);
    }
}