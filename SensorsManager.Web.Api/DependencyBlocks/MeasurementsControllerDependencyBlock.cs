using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class MeasurementsControllerDependencyBlock : IMeasurementsControllerDependencyBlock
    {
        public IMeasurementRepository MeasurementRepository { get; private set; }

        public MeasurementsControllerDependencyBlock(IMeasurementRepository measurementRepository)
        {
            MeasurementRepository = measurementRepository;
        }
    }
}