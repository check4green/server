using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using System.Net.Http;
using System.Web.Http.Routing;


namespace SensorsManager.Web.Api.Models
{
    public class ModelFactory
    {
        UrlHelper urlHelper;

        public ModelFactory(HttpRequestMessage httpRequestMessage)
        {
            urlHelper = new UrlHelper(httpRequestMessage);
        }
        public SensorModelGet CreateSensorModel(Sensor sensor)
        {
            return new SensorModelGet
            {
                Url = urlHelper.Link("GetSensorByAddressRoute",
                new
                {
                    gatewayAddress = sensor.GatewayAddress,
                    clientAddress = sensor.ClientAddress
                }),
                Name = sensor.Name,
                ProductionDate = sensor.ProductionDate,
                SensorTypeId = sensor.SensorTypeId,
                UploadInterval = sensor.UploadInterval,
                GatewayAddress = sensor.GatewayAddress,
                ClientAddress = sensor.ClientAddress,
                Latitude = sensor.Latitude,
                Longitude = sensor.Longitude,
                Active = sensor.Active
            };
        }

        public SensorPendingModel CreateSensorModel(
            int id, int uploadInterval)
        {
            return new SensorPendingModel
            {
                Id = id,
                UploadInterval = uploadInterval,
            };
        }
        public SensorTypeModel CreateSensorTypeModel(SensorType sensorType)
        {
            return new SensorTypeModel
            {
                Url = urlHelper.Link("GetSensorTypeByIdRoute", new { id = sensorType.Id }),
                Id = sensorType.Id,
                Code = sensorType.Code,
                Description = sensorType.Description,
                MaxValue = sensorType.MaxValue,
                MinValue = sensorType.MinValue,
                MeasureId = sensorType.MeasureId,
                Multiplier = sensorType.Multiplier
            };
        }
        public MeasurementModel CreateMeasurementModel(Measurement measurement)
        {
            return new MeasurementModel
            {
                Url = urlHelper.Link("GetMeasurementByIdRoute", new { id = measurement.Id }),
                Id = measurement.Id,
                UnitOfMeasure = measurement.UnitOfMeasure,
                Description = measurement.Description
            };
        }

        public SensorReadingModelGet CreateSensorReadingModel(SensorReading sensorReading)
        {
            return new SensorReadingModelGet
            {
                Id = sensorReading.Id,
                Value = sensorReading.Value,
                ReadingDate = sensorReading.ReadingDate
            };
        }

        public UserModelGet CreateUserModel(User user)
        {
            return new UserModelGet
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CompanyName = user.CompanyName,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber
            };
        }


    }
}