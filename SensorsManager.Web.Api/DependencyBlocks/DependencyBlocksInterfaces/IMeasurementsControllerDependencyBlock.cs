using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public interface IMeasurementsControllerDependencyBlock
    {
        IMeasurementRepository MeasurementRepository { get; }
    }
}
