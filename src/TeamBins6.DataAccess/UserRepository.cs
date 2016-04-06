using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    //public class UserRepository : IUserRepository
    //{
    //    public async Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var notificationTypeCode = notificationType.ToString();
    //            return await db.UserNotificationSubscriptions.Where(s => s.NotificationType.Code == notificationTypeCode)
    //                .Select(x => new UserDto
    //                {
    //                    EmailAddress = x.User.EmailAddress,
    //                    Id = x.User.Id,
    //                    Name = x.User.FirstName
    //                }).ToListAsync();
    //        }
    //    }
    //}
    public interface IUserRepository
    {
        Task<IEnumerable<TeamDto>> GetTeams(int userId);
        Task<UserAccountDto> GetUser(int id);
        Task<UserAccountDto> GetUser(string email);
        Task SetDefaultTeam(int userId, int teamId);
        Task SaveUserProfile(EditProfileVm userProfileVm);

        Task SaveDefaultIssueSettings(DefaultIssueSettings model);

        Task<int> CreateAccount(UserAccountDto userAccount);

        // Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType);
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
                await con.ExecuteAsync(q, new {@userId = userId, @teamId = teamId});
            }

        }

        public async Task SaveDefaultIssueSettings(DefaultIssueSettings model)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                   con.Query<int>(
                    " UPDATE TEAMMEMBER SET DEFAULTPROJECTID=@projectId WHERE TEAMID=@teamId AND MEMBERID=@userId",
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
            var q = @"SELECT [ID]
                      ,[FirstName] as Name                    
                      ,[EmailAddress],
                        Password
                      ,[Avatar] as GravatarUrl
                      ,[DefaultTeamID]
                    FROM [dbo].[User]
                    WHERE EmailAddress=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = await con.QueryAsync<UserAccountDto>(q, new {@id = email});
                return com.FirstOrDefault();
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
                var com = await con.QueryAsync<UserAccountDto>(q, new {@id = id});
                return com.FirstOrDefault();
            }

        }

        public async Task SaveUserProfile(EditProfileVm userProfileVm)
        {
            var q = @"UPDATE [User] SET FirstName=@name WHERE ID=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new {@userId = userProfileVm.Id, @name = userProfileVm.Name});
            }
        }

        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
            var q = @"SELECT T.ID,T.Name
                  FROM [dbo].[TeamMember] TM
                  INNER JOIN TEAM T ON TM.TeamID=T.ID                 
                  WHERE TM.MemberID=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<TeamDto>(q, new {@id = userId});
            }
        }

        public async Task<int> CreateAccount(UserAccountDto userAccount)
        {
            var q =
                @"INSERT INTO [dbo].[User](FirstName,EmailAddress,Password,CreatedDate) VALUES(@n,@e,@p,@dt);SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var ss=
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



    }
}