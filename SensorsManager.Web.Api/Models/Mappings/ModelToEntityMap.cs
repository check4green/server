using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using System;


namespace SensorsManager.Web.Api.Models
{
    public class ModelToEntityMap
    {
        public void MapSensorModelToSensorEntity(
          SensorPendingModel sensorPendingModel, Sensor sensor)
        {
            sensor.UploadInterval = sensorPendingModel.UploadInterval;
        }

        public void MapSensorModelToSensorEntity(SensorModelPut sensorModel, Sensor sensor)
        {
            sensor.Name = sensorModel.Name;
            sensor.UploadInterval = sensorModel.UploadInterval;
            sensor.Latitude = sensorModel.Latitude;
            sensor.Longitude = sensorModel.Longitude;
        }

        public Sensor MapSensorModelToSensorEntity(SensorModelPost sensorModel, int userId)
        {
            return new Sensor
            {
                Name = sensorModel.Name,
                SensorTypeId = sensorModel.SensorTypeId,
                ProductionDate = sensorModel.ProductionDate,
                UploadInterval = sensorModel.UploadInterval,
                GatewayAddress = sensorModel.GatewayAddress.ToLower(),
                ClientAddress = sensorModel.ClientAddress.ToLower(),
                Latitude = sensorModel.Latitude,
                Longitude = sensorModel.Longitude,
                UserId = userId
            };
        }

       public SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModelPostAddres sensorReadingModel,int sensorId)
        {
            return new SensorReading
            {
                SensorId = sensorId,
                Value = sensorReadingModel.Value,
                ReadingDate = sensorReadingModel.ReadingDate,
                InsertDate = DateTime.UtcNow
            };
        }

        public Measurement MapMeasurementModelToMeasurementEntity(MeasurementModel measurementModel)
        {
            return new Measurement
            {
                Description = measurementModel.Description,
                UnitOfMeasure = measurementModel.UnitOfMeasure

            };
        }
        public void MapMeasurementModelToMeasurementEntity(MeasurementModel measurementModel, Measurement measurement)
        {
            measurement.Description = measurementModel.Description;
            measurement.UnitOfMeasure = measurementModel.UnitOfMeasure;
        }

        public SensorType MapSensorTypeModelToSensorTypeEnrity(SensorTypeModel sensorTypeModel)
        {
            return new SensorType
            {
                Code = sensorTypeModel.Code,
                Description = sensorTypeModel.Description,
                MinValue = sensorTypeModel.MinValue,
                MaxValue = sensorTypeModel.MaxValue,
                MeasureId = sensorTypeModel.MeasureId,
                Multiplier = sensorTypeModel.Multiplier
            };
        }

        public void MapSensorTypeModelToSensorTypeEntity(SensorTypeModel sensorTypeModel, SensorType sensorType)
        {
            sensorType.Code = sensorTypeModel.Code;
            sensorType.Description = sensorTypeModel.Description;
            sensorType.MinValue = sensorTypeModel.MinValue;
            sensorType.MaxValue = sensorTypeModel.MaxValue;
            sensorType.MeasureId = sensorTypeModel.MeasureId;
            sensorType.Multiplier = sensorTypeModel.Multiplier;
        }

        public User MapUserModelToUserEntity(UserModel userModel)
        {
            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Password = userModel.Password,
                CompanyName = userModel.CompanyName ?? "",
                Country = userModel.Country,
                PhoneNumber = userModel.PhoneNumber
            };
        }

        public void MapUserModelToUserEntity(UserModel userModel, User result)
        {
            result.FirstName = userModel.FirstName;
            result.LastName = userModel.LastName;
            result.Email = userModel.Email;
            result.Password = userModel.Password;
            result.CompanyName = userModel.CompanyName ?? "";
            result.Country = userModel.Country;
            result.PhoneNumber = userModel.PhoneNumber;
        }
    }
}