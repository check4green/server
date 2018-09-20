using SensorsManager.DomainClasses;


namespace SensorsManager.Web.Api.Models
{
    interface IModelToEntityMap
    {
        Sensor MapSensorModelToSensorEntity(SensorModelPut sensorModel, Sensor sensor);
        Sensor MapSensorModelToSensorEntity(SensorModelPost sensorModel, int userId);
        SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModelPostAddres sensorReadingModel, int sensorId);
        SensorReading MapSensorReadingModelToSensorReadingEntity(SensorReadingModelPostId sensorReadingModel);
        Measurement MapMeasurementModelToMeasurementEntity(MeasurementModel measurementModel);
        Measurement MapMeasurementModelToMeasurementEntity(MeasurementModel measurementModel, Measurement measurement);
        SensorType MapSensorTypeModelToSensorTypeEnrity(SensorTypeModel sensorTypeModel);
        SensorType MapSensorTypeModelToSensorTypeEntity(SensorTypeModel sensorTypeModel, SensorType sensorType);
        User MapUserModelToUserEntity(UserModel userModel);
        User MapUserModelToUserEntity(UserModel userModel, User result);
    }
}
