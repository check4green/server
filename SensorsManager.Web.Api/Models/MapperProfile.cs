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
            CreateMap<Measurement, MeasurementModel>().ReverseMap()
                 .ForMember(x => x.Id, opt => opt.Ignore());

            //SensorType
            CreateMap<SensorType, SensorTypeModel>().ReverseMap()
                .ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<SensorTypeModelUpdate, SensorType>().ReverseMap();

            //Network
            CreateMap<NetworkModelPost, Network>()
                .ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<Network, NetworkModelGet>();
            CreateMap<NetworkModelPut, Network>();
            CreateMap<Network, NetworkWithSensorsModel>()
                .ForMember(x => x.Network, opt => opt.Ignore())
                .ForMember(x => x.Sensors, opt => opt.Ignore())
                .AfterMap((n, nm) => { nm.Network = n.Name; })
                .AfterMap((n, nm) => { nm.Sensors = n.Sensors.Select(s => s.Address).ToList();});

            //Sensor
            CreateMap<SensorModelPost, Sensor>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.LastReadingDate, opt => opt.Ignore())
                .ForMember(x => x.LastInsertDate, opt => opt.Ignore())
                .ForMember(x => x.Active, opt => opt.Ignore());
            CreateMap<Sensor, SensorModelGet>();
            CreateMap<SensorModelPut, Sensor>().ReverseMap();
            CreateMap<SensorPendingModel, Sensor>()
                    .ForMember(x => x.Id, opt => opt.Ignore()).ReverseMap();

            //Gateway
            CreateMap<GatewayModelPost, Gateway>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.LastSignalDate, opt => opt.Ignore())
                .ForMember(x => x.Active, opt => opt.Ignore())
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