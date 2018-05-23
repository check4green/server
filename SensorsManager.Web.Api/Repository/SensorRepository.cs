using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System;
using System.Data.Entity;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class SensorRepository
    {
        public Sensor AddSensor(Sensor sensor)
        {
            using (DataContext db = new DataContext())
            {
             
                var res = db.Sensors.Add(sensor);
                db.SaveChanges();
                return res;
            }
        }

        public Sensor GetSensorByAdress(string gatewayAdress, string clientAdress)
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType).Include(p => p.User)
                    .Where(p => p.GatewayAddress == gatewayAdress && p.ClientAddress == clientAdress)
                    .SingleOrDefault();
            }
        }

        public IQueryable<Sensor> GetSensosByGatewayAdress(string gatewayAdress)
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType).Include(p => p.User)
                    .Where(p => p.GatewayAddress == gatewayAdress)
                    .ToList().AsQueryable();
            }
        }

        public Sensor GetSensorById(int id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType).Include(p => p.User)
                    .Where(p => p.Id == id).SingleOrDefault();
            }
        }

        public IQueryable<Sensor> GetAllSensors()
        {
            using (DataContext db = new DataContext())
            {
                return db.Sensors.Include(p => p.SensorType).Include(p => p.User)
                    .ToList().AsQueryable();
            }
        }

        public void DeleteSensor(int id)
        {
            using (DataContext db = new DataContext())
            {
                var sensor = new Sensor() { Id = id };
                db.Entry(sensor).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public void DeleteSensorByAdress(string gatewayAdress, string clientAdress)
        {
            using (DataContext db = new DataContext())
            {
                var sensor = new Sensor()
                { GatewayAddress = gatewayAdress, ClientAddress = clientAdress};
                db.Entry(sensor).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }


        public void UpdateSensor(Sensor sensor)
        {
            using(DataContext db = new DataContext())
            {
                db.Entry(sensor).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

      
    }
}