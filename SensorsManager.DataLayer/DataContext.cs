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
        public DbSet<User> Users { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<GatewayConnection> GatewayConnections { get; set; }

        public DataContext()
            :base("DataContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MeasurementConfiguration());
            modelBuilder.Configurations.Add(new SensorTypeConfiguration());
            modelBuilder.Configurations.Add(new SensorConfiguration());
            modelBuilder.Configurations.Add(new SensorReadingConfiguration());
            modelBuilder.Configurations.Add(new NetworkConfiguration());
            modelBuilder.Configurations.Add(new GatewayConfiguration());
            modelBuilder.Configurations.Add(new GatewayConnectionConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
        }
    }
}
