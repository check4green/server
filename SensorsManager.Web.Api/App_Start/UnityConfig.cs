using SensorsManager.DataLayer;
using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
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
            container.RegisterType<IGuidService, GuidService>();
            container.RegisterType<IGatewayConnectionService, GatewayConnectionService>();
            container.RegisterType<DataContext, DataContext>();

            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<INetworkRepository, NetworkRepository>();
            container.RegisterType<IGatewayRepository, GatewayRepository>();
            container.RegisterType<IGatewayConnectionRepository, GatewayConnectionRepository>();
            container.RegisterType<IMeasurementRepository, MeasurementRepository>();
            container.RegisterType<ISensorTypesRepository, SensorTypesRepository>();
            container.RegisterType<ISensorRepository, SensorRepository>();
            container.RegisterType<ISensorReadingRepository, SensorReadingRepository>();

            container.RegisterType<IConnectionsControllerDependencyBlock, ConnectionsControllerDependencyBlock>();
            container.RegisterType<IGatewaysControllerDependencyBlock, GatewaysControllerDependencyBlock>();
            container.RegisterType<IMeasurementsControllerDependencyBlock, MeasurementsControllerDependencyBlock>();
            container.RegisterType<INetworksControllerDependencyBlock, NetworksControllerDependencyBlock>();
            container.RegisterType<IReadingsControllerDependencyBlock, ReadingsControllerDependencyBlock>();
            container.RegisterType<ISensorsControllerDependencyBlock, SensorsControllerDependencyBlock>();
            container.RegisterType<ISensorTypesControllerDependencyBlock, SensorTypesControllerDependencyBlock>();
            container.RegisterType<IUsersControllerDependencyBlock, UsersControllerDependencyBlock>();

                                                                                                      

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}