using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamBins.Common
{
    
    public class UserAccountDto : UserDto
    {
        
        public string Password { set; get; }      
        public int? DefaultTeamId { set; get; }
    }

    public class UserDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string GravatarUrl { get; set; }

        public double? TestVal { set; get; }
    }

    public class AppUser 
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string Password { set; get; }
    }
}