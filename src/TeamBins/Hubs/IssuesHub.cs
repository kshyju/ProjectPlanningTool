using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBins.Hubs
{
    public class IssuesHub : Hub
    {
        public Task SubscribeToTeam(int id)
        {
            return Groups.Add(Context.ConnectionId, id.ToString());
        }

    }
}
