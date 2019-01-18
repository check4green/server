using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorTypesRepository
    {
         SensorType AddSensorType(SensorType sensorType);
         SensorType GetSensorTypeById(int id);
         IQueryable<SensorType> GetAllSensorTypes();
         void DeleteSensorType(int id);
         void UpdateSensorType(SensorType sensorType);
    }
}
