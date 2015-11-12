using System;
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
       
        public int Save(UserAccountDto userAccount)
        {
            var userEntity = new User();
            userEntity.FirstName = userAccount.Name;
            userEntity.EmailAddress = userAccount.EmailAddress;
            userEntity.Avatar = userAccount.GravatarUrl;
            userEntity.CreatedDate = DateTime.UtcNow;

            db.Users.Add(userEntity);
            db.SaveChanges();
            return userEntity.ID;

        }
    }
}