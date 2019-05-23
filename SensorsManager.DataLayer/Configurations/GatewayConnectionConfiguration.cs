using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class GatewayConnectionConfiguration : EntityTypeConfiguration<GatewayConnection>
    {
        public GatewayConnectionConfiguration()
        {
            HasKey(c => new { c.Gateway_Id, c.Sensor_Id });
            HasRequired(c => c.Gateway)
                .WithMany(g => g.Connections)
                .HasForeignKey(c => c.Gateway_Id);
            Property(c => c.Sensor_Id)
                .IsRequired();
        }
    }
}