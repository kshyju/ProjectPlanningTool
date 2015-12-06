using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TeamEntitiesConn db;
        public AccountRepository()
        {
            db = new TeamEntitiesConn();
        }

        public UserAccountDto GetUser(int userId)
        {
            var user = db.Users.FirstOrDefault(s => s.ID == userId);
            if (user != null)
            {
                return new UserAccountDto
                {
                    Id = user.ID,
                    Name = user.FirstName,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    GravatarUrl = user.Avatar,
                    DefaultTeamId = user.DefaultTeamID
                };
            }
            return null;
        }
        public UserAccountDto GetUser(string email)
        {
            var user = db.Users.FirstOrDefault(s => s.EmailAddress == email);
            if (user != null)
            {
                return new UserAccountDto
                {
                    Id = user.ID,
                    Name = user.FirstName,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    GravatarUrl = user.Avatar,
                    DefaultTeamId = user.DefaultTeamID
                    //    (user.TeamMembers1.Any(f=>f.MemberID==user.Id)? user.TeamMembers1.FirstOrDefault(c=>c.MemberID==user.Id).TeamID:null)
                };
            }
            return null;
        }

        public bool DoesAccountExist(string email)
        {
            return db.Users.Any(s => s.EmailAddress == email);
        }

        public int Save(UserAccountDto userAccount)
        {
            var userEntity = new User();
            userEntity.FirstName = userAccount.Name;
            userEntity.EmailAddress = userAccount.EmailAddress;
            userEntity.Password = userAccount.Password;
            userEntity.Avatar = userAccount.GravatarUrl;
            userEntity.CreatedDate = DateTime.UtcNow;

            db.Users.Add(userEntity);
            db.SaveChanges();
            return userEntity.ID;

        }



        public async Task SaveLastLoginAsync(int userId)
        {
            var user = db.Users.FirstOrDefault(s => s.ID == userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public void SavePasswordResetRequest(UserAccountDto userAccount, string activationLink)
        {
            try
            {
                var request = new PasswordResetRequest { ActivationCode = activationLink, UserID = userAccount.Id };
                request.CreatedDate = DateTime.UtcNow;
                db.PasswordResetRequests.Add(request);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ResetPaswordRequestDto GetResetPaswordRequest(string id)
        {
            var request = db.PasswordResetRequests.FirstOrDefault(s => s.ActivationCode == id);
            if (request != null)
            {
                return new ResetPaswordRequestDto
                {
                    ActivationCode = request.ActivationCode,
                    UserId = request.UserID
                };
            }
            return null;
        }

        public void UpdatePassword(string password, int userId)
        {
            var user = db.Users.FirstOrDefault(s => s.ID == userId);
            if (user != null)
            {
                user.Password = password;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void UpdateProfile(UserAccountDto userAccountDto)
        {
            var user = db.Users.FirstOrDefault(s => s.ID == userAccountDto.Id);
            if (user != null)
            {
                user.FirstName = userAccountDto.Name;
                user.EmailAddress = userAccountDto.EmailAddress;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public UserEmailNotificationSettingsVM GetUserNotificationSettings(int userId,int teamId)
        {
            var userSubscriptions = new UserEmailNotificationSettingsVM();
            userSubscriptions.EmailSubscriptions = db.NotificationTypes.Select(s => new EmailSubscriptionVM {NotificationTypeID = s.ID, Name = s.Name}).ToList();

            var user = db.Users.FirstOrDefault(s => s.ID == userId);
            if (user != null && user.UserNotificationSubscriptions.Any())
            {
                var userSubScriptions = user.UserNotificationSubscriptions.ToList();
                foreach (var emailSubscriptionVm in userSubscriptions.EmailSubscriptions)
                {
                    emailSubscriptionVm.IsSelected =
                        userSubScriptions.Any(
                            s =>
                                s.NotificationTypeID == emailSubscriptionVm.NotificationTypeID && s.TeamID == teamId &&
                                s.Subscribed);
                }
                
            }
            return userSubscriptions;
        }

        public int? GetDefaultProjectForIssues(int userId, int teamId)
        {
            var teamMember =db.TeamMembers.FirstOrDefault(s => s.TeamID == teamId && s.MemberID == userId);
            return teamMember?.DefaultProjectID;
        }
    }
}