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

        public NetworksControllerDependencyBlock(
             IUserRepository userRepository,
             INetworkRepository networkRepository,
             ICredentialService credentialService,
             IGuidService guidService)
        {
            UserRepository = userRepository;
            NetworkRepository = networkRepository;
            CredentialService = credentialService;
            GuidService = guidService;
        }
    }
}