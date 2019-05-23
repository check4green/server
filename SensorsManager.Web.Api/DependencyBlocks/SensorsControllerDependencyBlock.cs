using System;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class SensorsControllerDependencyBlock : ISensorsControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }

        public INetworkRepository NetworkRepository { get; private set; }

        public ISensorTypesRepository TypesRepository { get; private set; }

        public ISensorRepository SensorRepository { get; private set; }

        public IGatewayConnectionRepository ConnectionRepository { get; private set; }

        public ICredentialService CredentialService { get; private set; }

        public IDateTimeService DateTimeService { get; private set; }

        public SensorsControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             ISensorTypesRepository typesRepository,
             ISensorRepository sensorRepository,
             IGatewayConnectionRepository connectionRepository,
             ICredentialService credentialService,
             IDateTimeService dateTimeService
            )
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            TypesRepository = typesRepository;
            SensorRepository = sensorRepository;
            ConnectionRepository = connectionRepository;
            CredentialService = credentialService;
            DateTimeService = dateTimeService;
        }
    }
}