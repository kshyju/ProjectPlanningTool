using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartPlan.ViewModels
{
    public class DashBoardVM
    {
        public IList<TeamVM> Teams { set; get; }

        public DashBoardVM()
        {
            Teams = new List<TeamVM>();

        }
    }
    public class TeamVM
    {
        public int ID { set; get; }
        public string Name { set; get; }
        
    }
}