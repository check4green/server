using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class SensorTypesControllerDependencyBlock : ISensorTypesControllerDependencyBlock
    {
        public IMeasurementRepository MeasurementRepository { get; private set; }
        public ISensorTypesRepository TypesRepository { get; private set; }
        public IMapper Mapper { get; private set; }
        public IMessageService MessageService { get; private set; }
        public SensorTypesControllerDependencyBlock(
                IMeasurementRepository measurementRepository,
                ISensorTypesRepository typesRepository,
                IMapper mapper,
                IMessageService messageService
            )
        {
            MeasurementRepository = measurementRepository;
            TypesRepository = typesRepository;
            Mapper = mapper;
            MessageService = messageService;
        }
    }
}