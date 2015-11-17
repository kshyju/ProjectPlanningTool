using TeamBins.Common;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public interface IUserAccountEmailManager
    {
        void SendResetPasswordEmail(UserAccountDto userAccount,string activationLink);
    }
}