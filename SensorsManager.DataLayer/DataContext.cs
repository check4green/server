using SensorsManager.DomainClasses;
using System.Data.Entity;

namespace SensorsManager.DataLayer
{
    public class DataContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<SensorType> SensorsTypes { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }

        public DataContext()
            :base("name=DataContext")
        {

        }
      
    }
}
