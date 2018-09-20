
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Models
{
    public interface IModelFactory
    {
         SensorModelGet CreateSensorModel(Sensor sensor);
         SensorTypeModel CreateSensorTypeModel(SensorType sensorType);
         MeasurementModel CreateMeasurementModel(Measurement measurement);
         SensorReadingModelGet CreateSensorReadingModel(SensorReading sensorReading);
         UserModelGet CreateUserModel(User user);
    }
}
