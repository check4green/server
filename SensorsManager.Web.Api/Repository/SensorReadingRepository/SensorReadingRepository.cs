using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Linq;


namespace SensorsManager.Web.Api.Repository
{
    public class SensorReadingRepository : ISensorReadingRepository
    {
        DataContext db;

        public SensorReadingRepository(DataContext dataContext)
        {
            db = dataContext;
        }
        public SensorReading Add(SensorReading sensorReading)
        {
            var res = db.SensorReadings.Add(sensorReading);
            db.SaveChanges();
            return res;
        }

        public IQueryable<SensorReading> Get(int id)
        {
            var readings = db.SensorReadings.Where(p => p.Sensor_Id == id)
                .ToList().AsQueryable();
            return readings;
        }
    }
}