using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorReadingRepository
    {
        SensorReading AddSensorReading(SensorReading sensorReading);
        IQueryable<SensorReading> GetSensorReadingBySensorId(int id);
    }
}
