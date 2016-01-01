using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace TeamBins.Common
{
    
    public class UserStore : IUserStore<AppUser>, IUserLoginStore<AppUser>
    {
        private readonly string connectionString;
       public UserStore()
       {
           this.connectionString = "";
       }
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(AppUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(AppUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(AppUser user)
        {
            throw new System.NotImplementedException();
        }

        Task<AppUser> IUserStore<AppUser, string>.FindByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        Task<AppUser> IUserStore<AppUser, string>.FindByNameAsync(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task FindByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task FindByNameAsync(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveLoginAsync(AppUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }


        public Task GetLoginsAsync(AppUser user)
        {
            throw new System.NotImplementedException();
        }
       // FindAsync

        public Task<AppUser> FindAsync(UserLoginInfo login)
        {
            throw new System.NotImplementedException();
        }

        Task<IList<UserLoginInfo>> IUserLoginStore<AppUser, string>.GetLoginsAsync(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
    public class UserAccountDto : UserDto
    {
        
        public string Password { set; get; }
        public string GravatarUrl { get; set; }

        public int? DefaultTeamId { set; get; }
    }

    public class UserDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string GravatarUrl { get; set; }
    }

    public class AppUser : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string Password { set; get; }
    }
}