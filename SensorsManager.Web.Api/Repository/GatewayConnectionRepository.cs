using System.Linq;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Repository
{
    public class GatewayConnectionRepository : IGatewayConnectionRepository
    {
        public GatewayConnection Add(GatewayConnection gatewayConnection)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.GatewayConnections.Add(gatewayConnection);
                db.SaveChanges();
                return res;
            }
        }

        public IQueryable<GatewayConnection> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.GatewayConnections.ToList().AsQueryable();
            }
        }

        public void Delete(int sensorId)
        {
            using(DataContext db = new DataContext())
            {
                db.GatewayConnections.RemoveRange(
                    db.GatewayConnections
                    .Where(p => p.Sensor_Id == sensorId));
            }
        }
    }
}