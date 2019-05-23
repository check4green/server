using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class UsersControllerDependencyBlock : IUsersControllerDependencyBlock
    {
        public IUserRepository UserRepository { get; private set; }

        public ICredentialService CredentialService { get; private set; }

        public IDateTimeService DateTimeService { get; private set; }

        public IMemCacheService MemCacheService { get; private set; }

        public IRandomService RandomService { get; private set; }

        public IMailSenderService MailSenderService { get; private set; }

        public UsersControllerDependencyBlock(
                IUserRepository userRepository,
                ICredentialService credentialService,
                IDateTimeService dateTimeService,
                IMemCacheService memCacheService,
                IRandomService randomService,
                IMailSenderService mailSenderService
            )
        {
            UserRepository = userRepository;
            CredentialService = credentialService;
            DateTimeService = dateTimeService;
            MemCacheService = memCacheService;
            RandomService = randomService;
            MailSenderService = mailSenderService;
        }
    }
}