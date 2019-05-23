using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorReadingRepository
    {
        SensorReading Add(SensorReading sensorReading);
        IQueryable<SensorReading> Get(int id);
    }
}
