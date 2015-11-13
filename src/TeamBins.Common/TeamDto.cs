using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamBins.Common
{
    public class LoggedInSessionInfo
    {
        public int TeamId { set; get; }
        public int UserId { set; get; }
        public string UserDisplayName { set;get; }
    }

    public class UserAccountDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public string GravatarUrl { get; set; }

        public int? DefaultTeamId { set; get; }
    }
    public class BaseEntityDto
    {
        public int Id { set; get; }
        public string Name { set; get; }
        
    }
    public class TeamDto : BaseEntityDto
    {
        public int CreatedById { get; set; }
    }
}
