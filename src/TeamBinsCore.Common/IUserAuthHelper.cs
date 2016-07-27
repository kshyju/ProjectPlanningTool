
namespace TeamBins.Common.ViewModels
{

    public interface IUserAuthHelper
    {
        int TeamId { get; }
        int UserId { get; }
        void SetTeamId(int teamId);
        void SetUserIDToSession(int userId, int teamId);
        void SetUserId(int userId);

        void SetUserIDToSession(LoggedInSessionInfo loggedInSessionInfo);

        void Logout();

    }
}