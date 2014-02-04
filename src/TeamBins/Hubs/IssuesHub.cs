using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace TeamBins
{
    public class IssuesHub : Hub
    {
        public Task SubscribeToTeam(int id)
        {
            return Groups.Add(Context.ConnectionId, id.ToString());
        }
        public System.Threading.Tasks.Task JoinRoom(int teamId)
        {
            //int userId= 

            return Groups.Add(Context.ConnectionId, teamId.ToString());
        }

    }
}