using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class GatewaysControllerDependencyBlock : IGatewaysControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }
        public INetworkRepository NetworkRepository { get; private set; }
        public IGatewayRepository GatewayRepository { get; private set; }
        public ICredentialService CredentialService { get; private set; }
        public IDateTimeService DateTimeService { get; private set; }
        public IMapper Mapper { get; private set; }
        public IMessageService MessageService { get; private set; }

        public GatewaysControllerDependencyBlock(IUserRepository userRepository,
             INetworkRepository networkRepository,
             IGatewayRepository gatewayRepository,
             ICredentialService credentialService,
             IDateTimeService dateTimeService,
             IMapper mapper,
             IMessageService messageService)
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            GatewayRepository = gatewayRepository;
            CredentialService = credentialService;
            DateTimeService = dateTimeService;
            Mapper = mapper;
            MessageService = messageService;
        }
    }
}