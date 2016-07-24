using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;
using TeamBins.CommonCore;

namespace TeamBins.DataAccessCore
{
    public interface IUserRepository
    {
        Task<IEnumerable<TeamDto>> GetTeams(int userId);
        Task<UserAccountDto> GetUser(int id);
        Task<UserAccountDto> GetUser(string email);
        Task SetDefaultTeam(int userId, int teamId);
        Task SaveUserProfile(EditProfileVm userProfileVm);
        Task SaveDefaultIssueSettings(DefaultIssueSettings model);
        Task<int> CreateAccount(UserAccountDto userAccount);
        Task<IEnumerable<EmailSubscriptionVM>> EmailSubscriptions(int userId, int teamId);
        Task SaveNotificationSettings(UserEmailNotificationSettingsVM model);
    }

}