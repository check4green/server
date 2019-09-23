using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class NetworkRepository : INetworkRepository
    {

        DataContext db;

        public NetworkRepository(DataContext dataContext)
        {
            db = dataContext;
        }

        public Network Add(Network network)
        {
            var res = db.Networks.Add(network);
            db.SaveChanges();
            return res;
        }

        public Network Get(int id)
        {
            return db.Networks
                .Include(n => n.Sensors)
                .SingleOrDefault(p => p.Id == id);
        }

        public IQueryable<Network> GetAll()
        {
            return db.Networks.ToList().AsQueryable();
        }

        public void Update(Network network)
        {
            db.Entry(network).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var network = new Network { Id = id };
            bool saveFailed;
            do
            {
                saveFailed = false;
                db.Entry(network).State = EntityState.Deleted;
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
            return db.Networks.Any(n => n.Id == id);
        }
    }
}