using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorRepository
    {
        Sensor Add(Sensor sensor);
        Sensor Get(int id);
        IQueryable<Sensor> GetAll();
        void Update(Sensor sensor);
        void Delete(int id);
        bool Exists(int id);
    }
}
