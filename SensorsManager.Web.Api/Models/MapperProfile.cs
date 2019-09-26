using AutoMapper;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using System.Linq;

namespace SensorsManager.Web.Api.Models
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //Measurement
            CreateMap<Measurement, MeasurementModelGet>();
            CreateMap<MeasurementModel, Measurement>().ReverseMap();

            //SensorType
            CreateMap<SensorType, SensorTypeModelGet>()
                .AfterMap((st, stm) => stm.MeasureId = st.Measure_Id);
            CreateMap<SensorTypeModelPost, SensorType>()
                .AfterMap((stm, st) => st.Measure_Id = stm.MeasureId);
            CreateMap<SensorTypeModelUpdate, SensorType>().ReverseMap();

            //Network
            CreateMap<Network, NetworkModelGet>();
            CreateMap<NetworkModel, Network>();
            CreateMap<Network, NetworkWithSensorsModel>()
                .ForMember(x => x.Network, opt => opt.Ignore())
                .ForMember(x => x.Sensors, opt => opt.Ignore())
                .AfterMap((n, nm) => { nm.Network = n.Name; })
                .AfterMap((n, nm) => { nm.Sensors = n.Sensors.Select(s => s.Address).ToList();});

            //Sensor
            CreateMap<SensorModelPost, Sensor>();
            CreateMap<Sensor, SensorModelGet>();
            CreateMap<SensorModelPut, Sensor>().ReverseMap();
            CreateMap<SensorPendingModel, Sensor>()
                    .ForMember(x => x.Id, opt => opt.Ignore()).ReverseMap();

            //Gateway
            CreateMap<GatewayModelPost, Gateway>()
                .AfterMap((gm, g) => g.UploadInterval = 5);
            CreateMap<Gateway, GatewayModelGet>();
            CreateMap<GatewayModelPut, Gateway>().ReverseMap();

            //Reading
            CreateMap<SensorReadingModelPost, SensorReading>();
            CreateMap<SensorReading, SensorReadingModelGet>();

            //User
            CreateMap<UserModelPost, User>();
            CreateMap<User, UserModelGet>();
            CreateMap<UserModelPut, User>().ReverseMap();
        }
    }
}