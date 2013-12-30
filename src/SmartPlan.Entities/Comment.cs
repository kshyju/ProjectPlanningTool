using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBins.Entities;
using TechiesWeb.TeamBins.Entities;

namespace Planner.Entities
{
    public class Comment
    {
        public int ID { set; get; }
        public string CommentBody { set; get; }
        public User Author { set; get; }
        public DateTime CreatedDate { set; get; }
        public int ParentID { set; get; }
        public Comment()
        {
            Author = new User();
        }
    }
}
