using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class SensorTypesControllerDependencyBlock : ISensorTypesControllerDependencyBlock
    {
        public IMeasurementRepository MeasurementRepository { get; private set; }

        public ISensorTypesRepository TypesRepository { get; private set; }

        public SensorTypesControllerDependencyBlock(
                IMeasurementRepository measurementRepository,
                ISensorTypesRepository typesRepository
            )
        {
            MeasurementRepository = measurementRepository;
            TypesRepository = typesRepository;
        }
    }
}