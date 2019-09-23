using AutoMapper;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Services;

namespace SensorsManager.Web.Api.DependencyBlocks
{
    public class MeasurementsControllerDependencyBlock : IMeasurementsControllerDependencyBlock
    {
        public IMeasurementRepository MeasurementRepository { get; private set; }
        public IMessageService MessageService { get; private set; }
        public IMapper Mapper { get; private set; }

        public MeasurementsControllerDependencyBlock(IMeasurementRepository measurementRepository,
            IMessageService messageService,
            IMapper mapper)
        {
            MeasurementRepository = measurementRepository;
            MessageService = messageService;
            Mapper = mapper;
        }
    }
}