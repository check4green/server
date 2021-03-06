﻿using System.Web;
using System.Web.Http;

namespace SensorsManager.Web.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ActivityManager.CheckActivity();
            //Ignore the self referencing loop
            GlobalConfiguration.Configuration
                .Formatters
                .JsonFormatter
                .SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        private IActivityManager ActivityManager
           => (IActivityManager)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IActivityManager));
    }
}
