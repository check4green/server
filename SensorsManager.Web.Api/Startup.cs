using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(SensorsManager.Web.Api.Startup))]

namespace SensorsManager.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.Map("/signalr", map =>
            {

                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {

                };

                map.RunSignalR(hubConfiguration);
            });

        }
    }
}
