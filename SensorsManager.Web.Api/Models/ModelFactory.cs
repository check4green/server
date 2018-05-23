using SensorsManager.DomainClasses;
using System;

namespace SensorsManager.Web.Api.Models
{
    public class ModelFactory
    {
       public SensorsModel CreateSensorsModel(Sensor sensor)
        {
            return new SensorsModel
            {
                Id = sensor.Id,
                BatchSize = sensor.BatchSize,
                ProductionDate = sensor.ProductionDate,
                SensorTypeId = sensor.SensorTypeId,
                UploadInterval = sensor.UploadInterval,
                GatewayAddress = sensor.GatewayAddress,
                ClientAddress = sensor.ClientAddress,
                UserId = sensor.UserId
            };
        }
       public SensorTypesModel CreateSensorTypesModel(SensorType sensorType)
        {
            return new SensorTypesModel
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
        public MeasurementsModel CreateMeasurementsModel(Measurement measurement)
        {
            return new MeasurementsModel
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
                SensorGatewayAdress = sensorReading.SensorGatewayAdress,
                SensorClientAdress = sensorReading.SensorClientAdress,
                Value = sensorReading.Value,
                ReadingDate = sensorReading.ReadingDate
            };
        }

        internal SensorReadingModel CreateSensorReadingModel(Sensor p)
        {
            throw new NotImplementedException();
        }
    }
}