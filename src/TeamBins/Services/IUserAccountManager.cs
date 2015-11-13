using TeamBins.Common;

namespace TeamBins.Services
{
    public interface IUserAccountManager
    {
        bool DoesAccountExist(string email);
        LoggedInSessionInfo CreateUserAccount(UserAccountDto userAccount);

        UserAccountDto GetUser(string email);
    }
}