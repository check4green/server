using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IMeasurementRepository
    {
        Measurement Add(Measurement measurement);
        Measurement Get(int id);
        IQueryable<Measurement> GetAll();
        void Delete(int id);
        void Update(Measurement measurement);
    }
}
