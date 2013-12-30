using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBins.Entities;
using TechiesWeb.TeamBins.Entities;

namespace Planner.Entities
{
    public class Bug :Issue
    {

        public int Cycle { set; get; }

        public Bug()
        {
            Project = new Project();
            Category = new Category();
            Status = new Status();
            Priority = new Priority();
            CreatedBy = new User();
            ModifiedBy = new User();
        }

    }
    public class Category
    {
        public int CategoryID { set; get; }
        public string CategoryName { set; get; }
    }
    public class Priority
    {
        public int PriorityID { set; get; }
        public string PriorityName { set; get; }
    }
}
