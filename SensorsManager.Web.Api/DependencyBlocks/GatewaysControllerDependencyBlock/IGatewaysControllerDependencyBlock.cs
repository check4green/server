using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IGatewaysControllerDependencyBlock
    {
        IUserRepository UserRepository { get; }
        INetworkRepository NetworkRepository { get; }
        IGatewayRepository GatewayRepository { get; }
        ICredentialService CredentialService { get; }
        IDateTimeService DateTimeService { get; }
        IMapper Mapper { get; }
        IMessageService MessageService { get; }
    }
}
