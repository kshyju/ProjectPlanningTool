using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBins.Entities;

namespace TechiesWeb.TeamBins.Entities
{
    public class Project : EntityBase
    {        
        public string Name { set; get; }
        public string Description { set; get; }
    
        public Team Team { set; get; }

        public Project()
        {
         
            Team = new Team();
        }
    }
}
