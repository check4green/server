using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorTypesRepository
    {
        SensorType Add(SensorType sensorType);
        SensorType Get(int id);
        IQueryable<SensorType> GetAll();
        void Update(SensorType sensorType);
        void Delete(int id);
        bool Exists(int id);
    }
}
