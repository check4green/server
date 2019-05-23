using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class MeasurementRepository : IMeasurementRepository
    {
        public Measurement Add(Measurement measurement)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.Measurements.Add(measurement);
                db.SaveChanges();
                return res;
            }
        }

        public Measurement Get(int id)
        {
            using(DataContext db = new DataContext())
            {
                return db.Measurements.Where(p => p.Id == id).SingleOrDefault();
            }
         }

        public IQueryable<Measurement> GetAll()
        {
            using(DataContext db = new DataContext())
            {
                return db.Measurements.ToList().AsQueryable();
            }
        }

        public void Update(Measurement measurement)
        {

            using (DataContext db = new DataContext())
            {

                db.Entry(measurement).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using(DataContext db = new DataContext())
            {
                var measure = new Measurement { Id = id };
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    db.Entry(measure).State = EntityState.Deleted;
                    try
                    {

                        db.SaveChanges();
                    }

                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        var entry = ex.Entries.Single();
                        if (entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                        }
                    }

                } while (saveFailed);
            }
        }

     
    }
}