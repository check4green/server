using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SensorsManager.Web.Api.Hubs
{

    public class ReadingsHub : Hub
    {
        
        public override Task OnConnected()
        {
            string name = Context.QueryString["address"];
            Groups.Add(Context.ConnectionId, name);

            return base.OnConnected();

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.QueryString["address"];
            Groups.Remove(Context.ConnectionId, name);

            return base.OnDisconnected(stopCalled);
        }
    }
}