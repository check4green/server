using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface ISensorTypesControllerDependencyBlock
    {
        IMeasurementRepository MeasurementRepository { get; }
        ISensorTypesRepository TypesRepository { get; }
        IMapper Mapper { get;  }
        IMessageService MessageService { get; }
    }
}
