using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface IAccountRepository
    {
        int Save(UserAccountDto userAccount);
        bool DoesAccountExist(string email);

        UserAccountDto GetUser(string email);
    }
}