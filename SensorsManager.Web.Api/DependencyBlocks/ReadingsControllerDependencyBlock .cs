using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class ReadingsControllerDependencyBlock : IReadingsControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }

        public INetworkRepository NetworkRepository { get; private set; }

        public IGatewayRepository GatewayRepository { get; private set; }

        public ISensorRepository SensorRepository { get; private set; }

        public IGatewayConnectionRepository ConnectionRepository { get; private set; }

        public ISensorReadingRepository ReadingRepository { get; private set; }

        public ICredentialService CredentialService { get; private set; }

        public IThrottlerService ThrottlerService { get; private set; }

        public IGatewayConnectionService ConnectionService { get; private set; }


        public ReadingsControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             IGatewayRepository gatewayRepository,
             ISensorRepository sensorRepository,
             IGatewayConnectionRepository connectionRepository,
             ISensorReadingRepository readingRepository,
             ICredentialService credentialService,
             IThrottlerService throttlerService,
             IGatewayConnectionService connectionService
            )
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            GatewayRepository = gatewayRepository;
            SensorRepository = sensorRepository;
            ConnectionRepository = connectionRepository;
            ReadingRepository = readingRepository;
            CredentialService = credentialService;
            ThrottlerService = throttlerService;
            ConnectionService = connectionService;
        }
    }
}