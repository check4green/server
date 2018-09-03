using SensorsManager.DomainClasses;
using System;

namespace SensorsManager.Web.Api.Models
{
    public class ModelFactory
    {
       public SensorModelGet CreateSensorModel(Sensor sensor)
        {
            return new SensorModelGet
            {
                Id = sensor.Id,
                Name = sensor.Name,
                BatchSize = sensor.BatchSize,
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
       public SensorTypeModel CreateSensorTypeModel(SensorType sensorType)
        {
            return new SensorTypeModel
            {
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