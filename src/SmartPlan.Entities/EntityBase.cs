using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechiesWeb.TeamBins.Entities
{
    public class EntityBase
    {
        public int ID { set; get; }
        public int SiteID { set; get; }
        public User CreatedBy { set; get; }

        public EntityBase()
        {
            CreatedBy = new User();
        }
    }
}