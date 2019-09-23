using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
using SensorsManager.Web.Api.Validations;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IReadingsControllerDependencyBlock
    {
        IUserRepository UserRepository { get; }
        INetworkRepository NetworkRepository { get; }
        IGatewayRepository GatewayRepository { get; }
        ISensorRepository SensorRepository { get; }
        IGatewayConnectionRepository ConnectionRepository { get; }
        ISensorReadingRepository ReadingRepository { get; }
        ICredentialService CredentialService { get; }
        IThrottlerService ThrottlerService { get; }
        IGatewayConnectionService ConnectionService { get; }
        IDateTimeService DateTimeService { get; }
        IVibrationFilter VibrationFilter { get; }
        IMapper Mapper { get; }
        IMessageService MessageService { get; }
    }
}
