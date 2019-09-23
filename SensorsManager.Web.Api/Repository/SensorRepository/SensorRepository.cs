using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class SensorRepository : ISensorRepository
    {
        DataContext db;
        public SensorRepository(DataContext dataContext)
        {
            db = dataContext;
        }
        public Sensor Add(Sensor sensor)
        {
            var res = db.Sensors.Add(sensor);
            db.SaveChanges();
            return res;
        }

        public Sensor Get(int id)
        {
            return db.Sensors.Include(p => p.SensorType)
                .Include(p => p.Network)
                .Where(p => p.Id == id).SingleOrDefault();
        }

        public IQueryable<Sensor> GetAll()
        {
            return db.Sensors.Include(p => p.SensorType)
                   .ToList().AsQueryable();
        }

        public void Update(Sensor sensor)
        {
            db.Entry(sensor).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
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
        public bool Exists(int id)
        {
            return db.Sensors.Any(s => s.Id == id);
        }
    }
}