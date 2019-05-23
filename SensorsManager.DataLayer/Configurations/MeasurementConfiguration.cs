using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class MeasurementConfiguration : EntityTypeConfiguration<Measurement>
    {
        public MeasurementConfiguration()
        {
            HasKey(m => m.Id);
            Property(m => m.UnitOfMeasure)
                .HasMaxLength(50)
                .IsRequired();
            Property(m => m.Description)
                .HasMaxLength(50)
                .IsOptional();
          
        }
    }
}