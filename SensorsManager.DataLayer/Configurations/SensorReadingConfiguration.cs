using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class SensorReadingConfiguration : EntityTypeConfiguration<SensorReading>
    {
        public SensorReadingConfiguration()
        {
            HasKey(r => r.Id);
            HasRequired(r => r.Sensor)
                .WithMany(s => s.SensorReadings)
                .HasForeignKey(r => r.Sensor_Id);
            Property(r => r.GatewayAddress)
                .IsFixedLength()
                .HasMaxLength(10)
                .IsOptional();
            Property(r => r.InsertDate)
                .IsRequired();
        }
    }
}