using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins
{
    public class IssuesHub : Hub
    {
        public Task SubscribeToTeam(int id)
        {
            return Groups.Add(Context.ConnectionId, id.ToString());
        }
       
    }
}