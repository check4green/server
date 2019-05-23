using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface ISensorTypesControllerDependencyBlock
    {
        IMeasurementRepository MeasurementRepository { get; }
        ISensorTypesRepository TypesRepository { get; }
    }
}
