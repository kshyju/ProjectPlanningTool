using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TeamEntities db;
        public AccountRepository()
        {
            db = new TeamEntities();
        }

        public UserAccountDto GetUser(string email)
        {
            var user = db.Users.FirstOrDefault(s => s.EmailAddress == email);
            if (user != null)
            {
                return new UserAccountDto
                {
                    Name = user.FirstName,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    GravatarUrl = user.Avatar,
                    DefaultTeamId = user.DefaultTeamID
                    //    (user.TeamMembers1.Any(f=>f.MemberID==user.ID)? user.TeamMembers1.FirstOrDefault(c=>c.MemberID==user.ID).TeamID:null)
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
    }
}