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
        public Sensor MapSensorModelToSensorEntity(SensorModel2 sensorModel, Sensor sensor)
        {
            sensor.Name = sensorModel.Name;
            sensor.UploadInterval = sensorModel.UploadInterval;
            sensor.BatchSize = sensorModel.BatchSize;
            return sensor;
        }

        public Sensor MapSensorModelToSensorEntity(SensorModel3 sensorModel)
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
                UserId = 1
            };
        }
       public SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModel2 sensorReadingModel,int sensorId)
        {
            return new SensorReading
            {
                SensorId = sensorId,
                Value = sensorReadingModel.Value,
                ReadingDate = sensorReadingModel.ReadingDate,
                InsertDate = DateTime.UtcNow
            };
        }

       public SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModel3 sensorReadingModel)
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
    }
}