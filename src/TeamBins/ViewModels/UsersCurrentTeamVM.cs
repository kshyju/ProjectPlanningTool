using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechiesWeb.TeamBins.ViewModels;

namespace TechiesWeb.TeamBins.ViewModels
{
    public class UserMenuHeaderVM
    {
        public List<TeamVM> Teams { set; get; }
        public int SelectedTeam { set; get; }
      //  public int CurrentTeamID { set; get; }
        public string CurrentTeamName { set; get; }
        public string UserDisplayName { set; get; }
        public string UserAvatarHash { set; get; }
        public UserMenuHeaderVM()
        {
            Teams = new List<TeamVM>();
        }
    }
   
}
