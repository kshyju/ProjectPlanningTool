using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBins.Entities;
using TechiesWeb.TeamBins.Entities;

namespace Planner.Entities
{
    public class Issue
    {
        public int ID { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public Project Project { set; get; }
        public Category Category { set; get; }
        public Status Status { set; get; }
        public Priority Priority { set; get; }
        public User CreatedBy { set; get; }
        public User ModifiedBy { set; get; }
        public bool IsShowStopper { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ModifiedDate { set; get; }
        public DateTime DueDate { set; get; }
        public string Iteration { set; get; }
        public Issue()
        {
            Project = new Project();
            Category = new Category();
            Status = new Status();
            Priority = new Priority();
            CreatedBy = new User();
            ModifiedBy = new User();
            Team = new Team();
        }
        public Team Team { set; get; }
    }
}
