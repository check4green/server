using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Repository
{
    public class GatewayRepository : IGatewayRepository
    {
        private readonly DataContext db;

        public GatewayRepository(DataContext dataContext)
        {
            db = dataContext;
        }
        public Gateway Add(Gateway gateway)
        {
            var res = db.Gateways.Add(gateway);
            db.SaveChanges();
            return res;
        }

        public Gateway Get(int id)
        {
            return db.Gateways.Where(p => p.Id == id)
                .Include(p => p.Network)
                .Include(p => p.Connections)
                .SingleOrDefault();
        }

        public Gateway Get(string address)
        {
            return db.Gateways.Where(p => p.Address == address).SingleOrDefault();
        }

        public IQueryable<Gateway> GetAll()
        {
            return db.Gateways.ToList().AsQueryable();
        }

        public void Update(Gateway gateway)
        {
            db.Entry(gateway).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
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
        public bool Exists(int id)
        {
            return db.Gateways.Any(g => g.Id == id);
        }
    }
}