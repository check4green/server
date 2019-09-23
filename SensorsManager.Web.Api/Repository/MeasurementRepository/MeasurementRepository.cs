using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class MeasurementRepository : IMeasurementRepository
    {
        DataContext db;

        public MeasurementRepository(DataContext dataContext)
        {
            db = dataContext;
        }

        public void Add(Measurement measurement)
        {     
            db.Measurements.Add(measurement);
            db.SaveChanges();
        }

        public Measurement Get(int id)
        {
            return db.Measurements.Where(p => p.Id == id).SingleOrDefault();
        }

        public IQueryable<Measurement> GetAll()
        {
            return db.Measurements.ToList().AsQueryable();
        }

        public void Update(Measurement measurement)
        {
             db.Entry(measurement).State = EntityState.Modified;
             db.SaveChanges();
        }

        public void Delete(int id)
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

        public bool Exists(int id)
        {
            return db.Measurements.Any(m => m.Id == id);
        }
    }
}