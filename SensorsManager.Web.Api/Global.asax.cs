using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SensorsManager.Web.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
           
        }
    }
}
