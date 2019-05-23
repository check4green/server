using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class SensorRepository : ISensorRepository
    {
        public Sensor Add(Sensor sensor)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.Sensors.Add(sensor);
                db.SaveChanges();
                return res;
            }
        }

        public Sensor Get(int id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType)
                    .Include(p => p.Network)
                    .Where(p => p.Id == id).SingleOrDefault();
            }
        }

        public IQueryable<Sensor> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType)
                    .ToList().AsQueryable();
            }
        }

        public void Update(Sensor sensor)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(sensor).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (DataContext db = new DataContext())
            {
                var sensor = new Sensor() { Id = id };
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    db.Entry(sensor).State = EntityState.Deleted;
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