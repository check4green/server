using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class ModelToEntityMap
    {
        public Sensor MapSensorModelToSensorEntity(SensorModelPut sensorModel, Sensor sensor)
        {
            sensor.Name = sensorModel.Name;
            sensor.UploadInterval = sensorModel.UploadInterval;
            sensor.BatchSize = sensorModel.BatchSize;
            sensor.Latitude = sensorModel.Latitude;
            sensor.Longitude = sensorModel.Longitude;
            return sensor;
        }

        public Sensor MapSensorModelToSensorEntity(SensorModelPost sensorModel, int userId)
        {
            return new Sensor
            {
                Name = sensorModel.Name,
                SensorTypeId = sensorModel.SensorTypeId,
                ProductionDate = sensorModel.ProductionDate,
                UploadInterval = sensorModel.UploadInterval,
                BatchSize = sensorModel.BatchSize,
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

       public SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModelPostId sensorReadingModel)
        {
            return new SensorReading
            {
                SensorId = sensorReadingModel.SensorId,
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
        public Measurement MapMeasurementModelToMeasurementEntity(MeasurementModel measurementModel, Measurement measurement)
        {
            measurement.Description = measurementModel.Description;
            measurement.UnitOfMeasure = measurementModel.UnitOfMeasure;

            return measurement;
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

        public SensorType MapSensorTypeModelToSensorTypeEntity(SensorTypeModel sensorTypeModel, SensorType sensorType)
        {
            sensorType.Code = sensorTypeModel.Code;
            sensorType.Description = sensorTypeModel.Description;
            sensorType.MinValue = sensorTypeModel.MinValue;
            sensorType.MaxValue = sensorTypeModel.MaxValue;
            sensorType.MeasureId = sensorTypeModel.MeasureId;
            sensorType.Multiplier = sensorTypeModel.Multiplier;

            return sensorType;
        }

    
        public User MapUserModel2ToUserEntity(UserModelPost userModel)
        {
            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Password = userModel.Password,
                CompanyName = userModel.CompanyName != null? userModel.CompanyName: "",
                Country = userModel.Country,
                PhoneNumber = userModel.PhoneNumber
            };
        }

        public User MapUserModel2ToUserEntity(UserModelPost userModel, User result)
        {
            result.FirstName = userModel.FirstName;
            result.LastName = userModel.LastName;
            result.Email = userModel.Email;
            result.Password = userModel.Password;
            result.CompanyName = userModel.CompanyName != null? userModel.CompanyName: "";
            result.Country = userModel.Country;
            result.PhoneNumber = userModel.PhoneNumber;

            return result;
        }
    }
}