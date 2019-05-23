using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class SensorTypeConfiguration : EntityTypeConfiguration<SensorType>
    {
        public SensorTypeConfiguration()
        {
            HasKey(st => st.Id);
            HasRequired(st => st.Measurement)
                .WithMany(m => m.SensorType)
                .HasForeignKey(st => st.Measure_Id);
            Property(st => st.Name)
                .HasMaxLength(50)
                .IsRequired();
            Property(st => st.Description)
                .HasMaxLength(100)
                .IsOptional();
            Property(st => st.MinValue).IsRequired();
            Property(st => st.MaxValue).IsRequired();
        }
    }
}