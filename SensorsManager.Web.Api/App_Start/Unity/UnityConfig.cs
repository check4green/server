using AutoMapper;
using SensorsManager.DataLayer;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
using SensorsManager.Web.Api.Validations;
using System;
using Unity;

namespace SensorsManager.Web.Api
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            //Services
            container.RegisterType<IMemCacheService, MemCacheService>();
            container.RegisterType<IMailSenderService, MailSenderService>();
            container.RegisterType<IDateTimeService, DateTimeService>();
            container.RegisterType<IRandomService, RandomService>();
            container.RegisterType<IThrottlerService, ThrottlerService>();
            container.RegisterType<ICredentialService, CredentialService>();
            container.RegisterType<IGuidService, GuidService>();
            container.RegisterType<IGatewayConnectionService, GatewayConnectionService>();
            container.RegisterType<IMessageService, MessageService>();

            //Repository
            container.RegisterType<DataContext, DataContext>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<INetworkRepository, NetworkRepository>();
            container.RegisterType<IGatewayRepository, GatewayRepository>();
            container.RegisterType<IGatewayConnectionRepository, GatewayConnectionRepository>();
            container.RegisterType<IMeasurementRepository, MeasurementRepository>();
            container.RegisterType<ISensorTypesRepository, SensorTypesRepository>();
            container.RegisterType<ISensorRepository, SensorRepository>();
            container.RegisterType<ISensorReadingRepository, SensorReadingRepository>();

            //Other
            container.RegisterType<IUserLogIn, UserLogIn>();
            container.RegisterType<IVibrationFilter, VibrationFilter>();
            container.RegisterType<IActivityManager, ActivityManager>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile()));
            var mapper = config.CreateMapper();
            container.RegisterInstance(mapper);
        }
    }
}