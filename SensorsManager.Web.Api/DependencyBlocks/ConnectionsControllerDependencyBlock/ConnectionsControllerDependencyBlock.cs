using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

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
        public IMapper Mapper { get; private set; }
        public IMessageService MessageService { get; private set; }

        public ConnectionsControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             IGatewayRepository gatewayRepository,
             ISensorRepository sensorRepository,
             IGatewayConnectionRepository connectionRepository,
             ICredentialService credentialService,
             IMapper mapper,
             IMessageService messageService
            )
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            GatewayRepository = gatewayRepository;
            SensorRepository = sensorRepository;
            ConnectionRepository = connectionRepository;
            CredentialService = credentialService;
            Mapper = mapper;
            MessageService = messageService;
        }

    }
}