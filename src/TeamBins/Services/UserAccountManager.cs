using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class UserAccountManager : IUserAccountManager
    {
        readonly IAccountRepository accountRepository;
        public UserAccountManager(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }
        public int SaveUser(UserAccountDto userAccount)
        {
            userAccount.GravatarUrl = UserService.GetGravatarHash(userAccount.EmailAddress);
            return accountRepository.Save(userAccount);
        }
    }
}