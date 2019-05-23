using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Repository
{
    public class GatewayRepository : IGatewayRepository
    {
        public Gateway Add(Gateway gateway)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.Gateways.Add(gateway);
                db.SaveChanges();
                return res;
            }
        }

        public Gateway Get(int id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Gateways.Where(p => p.Id == id)
                    .Include(p => p.Network)
                    .Include(p => p.Connections)
                    .SingleOrDefault();
            }
        }

        public Gateway Get(string address)
        {
            using (DataContext db = new DataContext())
            {
                return db.Gateways.Where(p => p.Address == address).SingleOrDefault();
            }
        }

        public IQueryable<Gateway> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Gateways.ToList().AsQueryable();
            }
        }

        public void Update(Gateway gateway)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(gateway).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (DataContext db = new DataContext())
            {
                var gateway = new Gateway { Id = id };
                bool savedFailed;
                do
                {
                    savedFailed = false;
                    db.Entry(gateway).State = EntityState.Deleted;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch(DbUpdateConcurrencyException ex)
                    {
                        savedFailed = true;
                        var entry = ex.Entries.Single();
                        if(entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                        }
                    }
                } while (savedFailed);
                
            }
        }

    }
}