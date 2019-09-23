using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

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
        IMapper Mapper { get; }
        IMessageService MessageService { get;  }
    }
}
