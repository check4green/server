using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IMeasurementsControllerDependencyBlock
    {
        IMeasurementRepository MeasurementRepository { get; }
        IMessageService MessageService { get; }
        IMapper Mapper { get; }
    }
}
