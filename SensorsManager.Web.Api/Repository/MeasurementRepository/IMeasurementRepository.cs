using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IMeasurementRepository
    {
        void Add(Measurement measurement);
        Measurement Get(int id);
        IQueryable<Measurement> GetAll();
        void Update(Measurement measurement);
        void Delete(int id);
        bool Exists(int id);
    }
}
