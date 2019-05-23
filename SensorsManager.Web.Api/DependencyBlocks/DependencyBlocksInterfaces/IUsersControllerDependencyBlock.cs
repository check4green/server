using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IUsersControllerDependencyBlock
    {
        IUserRepository UserRepository { get; }
        ICredentialService CredentialService { get; }
        IDateTimeService DateTimeService { get; }
        IMemCacheService MemCacheService { get; }
        IRandomService RandomService { get; }
        IMailSenderService MailSenderService { get; }
    }
}
