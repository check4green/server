using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class SensorConfiguration : EntityTypeConfiguration<Sensor>
    {
        public SensorConfiguration()
        {
            HasKey(s => s.Id);
            HasRequired(s => s.SensorType)
                .WithMany(st => st.Sensors)
                .HasForeignKey(s => s.SensorType_Id);
            HasRequired(s => s.Network)
                .WithMany(n => n.Sensors)
                .HasForeignKey(s => s.Network_Id);
            Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();
            Property(s => s.ProductionDate).IsRequired();
            Property(s => s.UploadInterval).IsRequired();
            Property(s => s.Address)
                .HasMaxLength(10)
                .IsRequired();
            Property(s => s.Latitude).IsOptional();
            Property(s => s.Longitude).IsOptional();
        }
    }
}