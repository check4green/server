using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IConnectionsControllerDependencyBlock
    {
        IUserRepository UserRepository { get; }
        INetworkRepository NetworkRepository { get; }
        IGatewayRepository GatewayRepository { get; }
        ISensorRepository SensorRepository { get; }
        IGatewayConnectionRepository ConnectionRepository { get; }
        ICredentialService CredentialService { get; }
    }
}
