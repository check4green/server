using System.Linq;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Repository
{
    public class GatewayConnectionRepository : IGatewayConnectionRepository
    {
        DataContext db;

        public GatewayConnectionRepository(DataContext dataContext)
        {
            db = dataContext;
        }

        public GatewayConnection Add(GatewayConnection gatewayConnection)
        { 
            var res = db.GatewayConnections.Add(gatewayConnection);
            db.SaveChanges();
            return res; 
        }

        public IQueryable<GatewayConnection> GetAll()
        {
            return db.GatewayConnections.ToList().AsQueryable(); 
        }

        public void Delete(int sensorId)
        {
            db.GatewayConnections.RemoveRange(
                db.GatewayConnections
                .Where(p => p.Sensor_Id == sensorId));
        }

        public bool Exists(int sensorId)
        {
            return db.GatewayConnections.Any(gc => gc.Sensor_Id == sensorId);
        }
    }
}