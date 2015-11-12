using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamBins.Common
{
    public class UserAccountDto
    {
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public string GravatarUrl { get; set; }
    }
    public class BaseEntityDto
    {
        public int Id { set; get; }
        public string Name { set; get; }
        
    }
    public class TeamDto : BaseEntityDto
    {

    }
}
