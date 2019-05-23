using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class ConnectionsControllerDependencyBlock : IConnectionsControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }

        public INetworkRepository NetworkRepository { get; private set; }

        public IGatewayRepository GatewayRepository { get; private set; }

        public ISensorRepository SensorRepository { get; private set; }

        public IGatewayConnectionRepository ConnectionRepository { get; private set; }

        public ICredentialService CredentialService { get; private set; }

        public ConnectionsControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             IGatewayRepository gatewayRepository,
             ISensorRepository sensorRepository,
             IGatewayConnectionRepository connectionRepository,
             ICredentialService credentialService
            )
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            GatewayRepository = gatewayRepository;
            SensorRepository = sensorRepository;
            ConnectionRepository = connectionRepository;
            CredentialService = credentialService;
        }

    }
}