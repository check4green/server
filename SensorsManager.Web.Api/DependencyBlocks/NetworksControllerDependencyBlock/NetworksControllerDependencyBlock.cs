using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class NetworksControllerDependencyBlock : INetworksControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }
        public INetworkRepository NetworkRepository { get; private set; }
        public ICredentialService CredentialService { get; private set; }
        public IGuidService GuidService { get; private set; }
        public IDateTimeService DateTimeService { get; private set; }
        public IMapper Mapper { get; private set; }
        public IMessageService MessageService { get; private set; }
        public NetworksControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             ICredentialService credentialService,
             IGuidService guidService,
             IDateTimeService dateTimeService,
             IMapper mapper,
             IMessageService messageService)
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            CredentialService = credentialService;
            GuidService = guidService;
            DateTimeService = dateTimeService;
            Mapper = mapper;
            MessageService = messageService;
        }
    }
}