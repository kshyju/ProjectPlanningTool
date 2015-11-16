using System;
using System.Linq;
using TeamBins.Common;

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
    }
}