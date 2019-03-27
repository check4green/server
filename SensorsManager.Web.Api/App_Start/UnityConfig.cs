using SensorsManager.DataLayer;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Security.ServiceInterfaces;
using SensorsManager.Web.Api.ServiceInterfaces;
using SensorsManager.Web.Api.Services;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace SensorsManager.Web.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IMemCacheService, MemCacheService>();
            container.RegisterType<IMailSenderService, MailSenderService>();
            container.RegisterType<IDateTimeService, DateTimeService>();
            container.RegisterType<IRandomService, RandomService>();
            container.RegisterType<IThrottlerService, ThrottlerService>();
            container.RegisterType<ICredentialService, CredentialService>();
            container.RegisterType<DataContext,DataContext>();

            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IMeasurementRepository,MeasurementRepository>();
            container.RegisterType<ISensorTypesRepository, SensorTypesRepository>();
            container.RegisterType<ISensorRepository, SensorRepository>();
            container.RegisterType<ISensorReadingRepository, SensorReadingRepository>();



            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}