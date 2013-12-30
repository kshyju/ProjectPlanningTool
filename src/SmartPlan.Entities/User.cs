using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechiesWeb.TeamBins.Entities
{
    public class User
    {
        public int ID { set; get; }
        public string DisplayName { set; get; }
        public string JobTitle { set; get; }
        public string EmailAddress { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime LastLoginDate { set; get; }
        public string Password { set; get; }
        public string PasswordSalt { set; get; }
        public int SiteID { set; get; }
    }
}
