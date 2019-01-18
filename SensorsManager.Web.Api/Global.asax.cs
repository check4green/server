using System.Web;
using System.Web.Http;

namespace SensorsManager.Web.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents();
            ActivityCheck.CheckSensorActivity();
        }

        
    }
}
