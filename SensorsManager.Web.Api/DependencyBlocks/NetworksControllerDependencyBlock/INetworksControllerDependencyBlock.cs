using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface INetworksControllerDependencyBlock
    {
        IUserRepository UserRepository { get; }
        INetworkRepository NetworkRepository { get; }
        ICredentialService CredentialService { get; }
        IGuidService GuidService { get; }
        IDateTimeService DateTimeService { get; }
        IMapper Mapper { get; }
        IMessageService MessageService { get; }
    }
}
