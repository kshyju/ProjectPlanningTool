using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechiesWeb.TeamBins.ViewModels;

namespace SmartPlan.ViewModels
{
    public class UsersCurrentTeamVM
    {
        //View model for the user to switch betwen different teams ( shows in the header menu, Layout.cshtml)
        public List<TeamVM> Teams { set; get; }
        public int SelectedTeam { set; get; }
        public string CurrentTeamName { set; get; }
        public UsersCurrentTeamVM()
        {
            Teams = new List<TeamVM>();
        }
    }
}
