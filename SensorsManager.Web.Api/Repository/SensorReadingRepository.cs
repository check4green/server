using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace SensorsManager.Web.Api.Repository
{
    public class SensorReadingRepository
    {
        public SensorReading AddSensorReading(SensorReading sensorReading)
        {
            using(DataContext db = new DataContext())
            {
                var res = db.SensorReadings.Add(sensorReading);
                db.SaveChanges();
                return res;
            }
      
        }

        public IQueryable<SensorReading> GetSensorReadingBySensorId(int id)
        {
            using(DataContext db = new DataContext())
            {
                var readings = db.SensorReadings.Where(p => p.SensorId == id)
                    .ToList().AsQueryable();
                return readings;
            }
        }

        
    }
}