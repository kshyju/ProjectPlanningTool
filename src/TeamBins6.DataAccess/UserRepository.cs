using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dapper;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;

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
        Task SetDefaultTeam(int userId, int teamId);
        // Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType);
    }

    public class UserRepository : BaseRepo, IUserRepository
    {
        public async Task SetDefaultTeam(int userId, int teamId)
        {
            var q = @"UPDATE [User] SET DefaultTeamId=@teamId WHERE ID=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync(q, new { @userId = userId,@teamId=teamId });
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

        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
           var q = @"SELECT T.ID,T.Name
                  FROM [dbo].[TeamMember] TM
                  INNER JOIN TEAM T ON TM.TeamID=T.ID                 
                  WHERE TM.MemberID=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<TeamDto>(q, new {@id = userId });               
            }            
        }
        
    }



}