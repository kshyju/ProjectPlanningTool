using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType);
    }
}