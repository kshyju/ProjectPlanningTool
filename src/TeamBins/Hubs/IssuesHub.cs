using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins
{
    public class IssuesHub : Hub
    {
        public void NewTeamActivity(ActivityVM activity)
        {
            Clients.All.addNewTeamActivity(activity);
        }
        public System.Threading.Tasks.Task JoinRoom(int teamId)
        {
            //int userId= 

            return Groups.Add(Context.ConnectionId, teamId.ToString());
        }

    }
}