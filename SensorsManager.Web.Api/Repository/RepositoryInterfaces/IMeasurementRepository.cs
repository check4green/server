using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IMeasurementRepository
    {
        Measurement AddMeasurement(Measurement measurement);
        Measurement GetMeasurementById(int id);
        IQueryable<Measurement> GetAllMeasurements();
        void DeleteMeasurement(int id);
        void UpdateMeasurement(Measurement measurement);
    }
}
