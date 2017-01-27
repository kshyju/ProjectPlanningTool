using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using TeamBins.Common.ViewModels;


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

        Task UpdateLastLoginTime(int userId);
        Task SavePasswordResetRequest(PasswordResetRequest passwordResetRequest);
        Task<PasswordResetRequest> GetPasswordResetRequest(string activationCode);

        Task UpdatePassword(string password, int userId);

        Task<IEnumerable<UserDto>> GetAllUsers();
    }

    public class UserRepository : BaseRepo, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task SetDefaultTeam(int userId, int teamId)
        {
            var q = @"UPDATE [User] SET DefaultTeamId=@teamId WHERE ID=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new { @userId = userId, @teamId = teamId });
            }
        }

        public async Task SaveDefaultIssueSettings(DefaultIssueSettings model)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                con.Query<int>("UPDATE TEAMMEMBER SET DEFAULTPROJECTID=@projectId WHERE TEAMID=@teamId AND MEMBERID=@userId",
                 new
                 {
                     @projectId = model.SelectedProject.Value,
                     @teamId = model.TeamId,
                     @userId = model.UserId
                 });
            }
        }

        public async Task<UserAccountDto> GetUser(string email)
        {
            var q = @"SELECT [ID],[FirstName] as Name ,[EmailAddress],Password,[Avatar] as GravatarUrl ,[DefaultTeamID]
                    FROM [dbo].[User] 
                    WHERE EmailAddress=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = await con.QueryAsync<UserAccountDto>(q, new { @id = email });
                return com.FirstOrDefault();
            }
        }

        public async Task UpdateLastLoginTime(int userId)
        {
            var q = @"UPDATE [User] SET LastLoginDate=@date WHERE Id=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = await con.ExecuteAsync(q, new { @date = DateTime.UtcNow,userId });
                
            }
        }

        public async Task<UserAccountDto> GetUser(int id)
        {
            var q = @"SELECT [ID]
                      ,[FirstName] as Name                    
                      ,[EmailAddress]  
                      ,[Avatar] as GravatarUrl
                      ,[DefaultTeamID]
                    FROM [dbo].[User]
                    WHERE Id=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = await con.QueryAsync<UserAccountDto>(q, new { @id = id });
                return com.FirstOrDefault();
            }

        }

        public async Task UpdatePassword(string password, int userId)
        {
            var q = @"UPDATE [User] SET Password=@password WHERE ID=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new { password, userId });
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var q = @"SELECT [ID],[FirstName] as Name ,[EmailAddress],[Avatar] as GravatarUrl ,[DefaultTeamID],LastLoginDate,CreatedDate
                    FROM [dbo].[User] WITH (NOLOCK) ORDER BY CreatedDate desc";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<UserAccountDto>(q);
              
            }
        }

        public async Task SaveUserProfile(EditProfileVm userProfileVm)
        {
            var q = @"UPDATE [User] SET FirstName=@name WHERE ID=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new { @userId = userProfileVm.Id, @name = userProfileVm.Name });
            }
        }

        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
            var q = @"SELECT T.ID,T.Name
                  FROM [dbo].[TeamMember] TM
                  INNER JOIN TEAM T ON TM.TeamId=T.ID                 
                  WHERE TM.MemberID=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<TeamDto>(q, new { @id = userId });
            }
        }

        public async Task<int> CreateAccount(UserAccountDto userAccount)
        {
            var q =
                @"INSERT INTO [dbo].[User](FirstName,EmailAddress,Password,CreatedDate) VALUES(@n,@e,@p,@dt);SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var ss =
                    await
                        con.QueryAsync<int>
                        (q,
                            new
                            {
                                @n = userAccount.Name,
                                @e = userAccount.EmailAddress,
                                @p = userAccount.Password,
                                @dt = DateTime.Now
                            });
                return ss.Single();
            }
        }

        public async Task<IEnumerable<EmailSubscriptionVM>> EmailSubscriptions(int userId, int teamId)
        {
            var q = @"SELECT NT.ID as NotificationTypeId
                    ,[Code]
                    ,[Name],
                    ISNULL(UNS.Subscribed,0) as IsSelected 
                    FROM [dbo].[NotificationType] NT WITH (NOLOCK)
                    LEFT JOIN
                    UserNotificationSubscription UNS  WITH (NOLOCK) ON NT.ID=UNS.NotificationTypeID
                    AND UNS.UserID=@userId and UNS.TeamId=@teamId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<EmailSubscriptionVM>(q, new { userId, teamId });

            }
        }

        public async Task<PasswordResetRequest> GetPasswordResetRequest(string activationCode)
        {
            var q = @"SELECT PR.[ID],[UserID] ,[ActivationCode], U.Id, U.FirstName Name      
                  FROM [dbo].[PasswordResetRequest] PR WITH (NOLOCK)
                  JOIN dbo.[User] U  WITH (NOLOCK) ON U.ID =Pr.ID
                  WHERE PR.ActivationCode=@activationCode";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = await con.QueryAsync<PasswordResetRequest>(q, new { activationCode });
                return com.FirstOrDefault();
            }
        }

        public async Task SavePasswordResetRequest(PasswordResetRequest passwordResetRequest)
        {
            const string q = @"INSERT INTO [dbo].[PasswordResetRequest](UserId,ActivationCode,CreatedDate) VALUES(@UserId,@ActivationCode,@CreatedDate)";
            passwordResetRequest.CreatedDate = DateTime.UtcNow;
            using (var con = new SqlConnection(ConnectionString))
            {
                await con.ExecuteAsync(q,passwordResetRequest);
            }
        }
        public async Task SaveNotificationSettings(UserEmailNotificationSettingsVM model)
        {
            //DELETE EXISTING 
            var q = @"DELETE FROM UserNotificationSubscription WHERE UserId=@userId and TeamId=@teamId;";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new { @userId = model.UserId, @teamId = model.TeamId });

            }
            //Insert new
            foreach (var setting in model.EmailSubscriptions.Where(s => s.IsSelected))
            {
                var q2 = @"INSERT INTO
                    UserNotificationSubscription(UserID,NotificationTypeID,TeamId,Subscribed,ModifiedDate) VALUES
                    (@userId,@notificationTypeId,@teamId,@subscibed,@dt)";
                using (var con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    await con.ExecuteAsync(q2, new
                    {
                        @userId = model.UserId,
                        @subscibed = true,
                        @notificationTypeId = setting.NotificationTypeId,
                        @dt = DateTime.Now,
                        @teamId = model.TeamId
                    });

                }
            }

        }
    }

}