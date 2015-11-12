using TeamBins.Common;

namespace TeamBins.Services
{
    public interface IUserAccountManager
    {
        int SaveUser(UserAccountDto userAccount);
    }
}