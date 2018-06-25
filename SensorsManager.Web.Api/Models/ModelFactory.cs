using SensorsManager.DomainClasses;
using System;

namespace SensorsManager.Web.Api.Models
{
    public class ModelFactory
    {
       public SensorModel CreateSensorModel(Sensor sensor)
        {
            return new SensorModel
            {
                Id = sensor.Id,
                BatchSize = sensor.BatchSize,
                ProductionDate = sensor.ProductionDate,
                SensorTypeId = sensor.SensorTypeId,
                UploadInterval = sensor.UploadInterval,
                GatewayAddress = sensor.GatewayAddress,
                ClientAddress = sensor.ClientAddress,
                Activ = sensor.Activ
               
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

        public SensorReadingModel CreateSensorReadingModel(SensorReading sensorReading)
        {
            return new SensorReadingModel
            {
                Id = sensorReading.Id,
                Value = sensorReading.Value,
                ReadingDate = sensorReading.ReadingDate
            };
        }

    }
}