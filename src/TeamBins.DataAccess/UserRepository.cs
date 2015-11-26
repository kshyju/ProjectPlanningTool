using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;

namespace TeamBins.DataAccess
{
    public class UserRepository : IUserRepository
    {
        public Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType)
        {
            using (var db = new TeamEntitiesConn())
            {
                var notificationTypeCode = notificationType.ToString();
                return db.UserNotificationSubscriptions.Where(s => s.NotificationType.Code == notificationTypeCode)
                    .Select(x => new UserDto
                    {
                        EmailAddress = x.User.EmailAddress,
                        Id = x.User.ID,
                        Name = x.User.FirstName
                    }).ToListAsync();
            }
        }
    }
    public interface IUserRepository
    {
        Task<List<UserDto>> GetSubscribers(int teamId, NotificationTypeCode notificationType);
    }
}