using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using System.Collections.Generic;
using System.Linq;

namespace SensorsManager.Web.Api.Models
{
    public class ModelFactory
    {
        public SensorModelGet CreateModel(Sensor sensor)
        {
            return new SensorModelGet
            {
                Id = sensor.Id,
                Name = sensor.Name,
                ProductionDate = sensor.ProductionDate,
                LastReadingDate = sensor.LastReadingDate,
                SensorTypeId = sensor.SensorType_Id,
                UploadInterval = sensor.UploadInterval,
                Address = sensor.Address,
                Latitude = sensor.Latitude,
                Longitude = sensor.Longitude,
                Active = sensor.Active
            };
        }

        public SensorPendingModel CreateModel(
            int id, int uploadInterval)
        {
            return new SensorPendingModel
            {
                Id = id,
                UploadInterval = uploadInterval,
            };
        }
        public SensorTypeModel CreateModel(SensorType sensorType)
        {
            return new SensorTypeModel
            {

                Id = sensorType.Id,
                Name = sensorType.Name,
                Description = sensorType.Description,
                MaxValue = sensorType.MaxValue,
                MinValue = sensorType.MinValue,
                MeasureId = sensorType.Measure_Id,
            };
        }
        public MeasurementModel CreateModel(Measurement measurement)
        {
            return new MeasurementModel
            {
                Id = measurement.Id,
                UnitOfMeasure = measurement.UnitOfMeasure,
                Description = measurement.Description
            };
        }

        public SensorReadingModelGet CreateModel(SensorReading sensorReading)
        {
            return new SensorReadingModelGet
            {
                Id = sensorReading.Id,
                GatewayAddress = sensorReading.GatewayAddress,
                Value = sensorReading.Value,
                ReadingDate = sensorReading.ReadingDate
            };
        }

        public UserModelGet CreateModel(User user)
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

        public NetworkModelGet CreateModel(Network network)
        {
            return new NetworkModelGet
            {
                Id = network.Id,
                Address = network.Address,
                Name = network.Name
            };
        }

        public NetworkWithSensorsModel CreateModel(string network, List<Sensor> sensors)
        {
            return new NetworkWithSensorsModel()
            {
                Network = network,
                Sensors = sensors.Select(s => s.Address).ToList()
            };
        }


        public GatewayModelGet CreateModel(Gateway gateway)
        {
            return new GatewayModelGet
            {
                Id = gateway.Id,
                Address = gateway.Address,
                Name = gateway.Name,
                LastSensorDate = gateway.LastSensorDate,
                Network_Id = gateway.Network_Id,
                Latitude = gateway.Latitude,
                Longitude = gateway.Longitude,
                Active = gateway.Active
            };
        }
    }
}