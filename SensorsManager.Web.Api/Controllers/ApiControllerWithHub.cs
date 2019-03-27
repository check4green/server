using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace SensorsManager.Web.Api.Controllers
{
    public class ApiControllerWithHub<THub> : BaseApiController
        where THub : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
            );

        protected IHubContext Hub
        {
            get { return hub.Value; }
        }
    }
}
