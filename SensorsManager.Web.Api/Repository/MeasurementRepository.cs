using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Linq;

namespace SensorsManager.DataLayer
{
    public class MeasurementRepository
    {
        public Measurement AddMeasurement(Measurement measurement)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.Measurements.Add(measurement);
                db.SaveChanges();
                return res;
            }
        }

        public Measurement GetMeasurementById(int id)
        {
            using(DataContext db = new DataContext())
            {
                return db.Measurements.Where(p => p.Id == id).SingleOrDefault();
            }
         }

        public IQueryable<Measurement> GetAllMeasurements()
        {
            using(DataContext db = new DataContext())
            {
                return db.Measurements.ToList().AsQueryable();
            }
        }

        public void DeleteMeasurement(int id)
        {
            using(DataContext db = new DataContext())
            {
                var measure = new Measurement { Id = id };
                db.Entry(measure).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

       public void UpdateMeasurement(Measurement measurement)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(measurement).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}