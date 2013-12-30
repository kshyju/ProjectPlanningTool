using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechiesWeb.TeamBins.Entities;

namespace TeamBins.Entities
{
    public class Team
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public DateTime CreatedDate { set; get; }
        public User CreatedBy { set; get; }

        public Team()
        {
            CreatedBy = new User();
        }
    }

    public class TeamMemberRequest
    {
        public string EmailAddress { set; get; }
        public string ActivationCode { set; get; }
        public Team Team { set; get; }
        public User CreatedBy { set; get; } 
        public TeamMemberRequest()
        {
            Team = new Team();
            CreatedBy = new User();
        }
    }

    

    public class Activity
    {
        public int ID { set; get; }    
        public User CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public int ItemID { set; get; }
        public string ItemType { set; get; }
        public string Action { set; get; }
        public string NewState { set; get; }
        public string ItemName { set; get; }
        public int TeamID { set; get; }
        public Activity()
        {
            CreatedBy = new User();            
        }

    }
    
}
