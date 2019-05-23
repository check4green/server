using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class GatewayConfiguration : EntityTypeConfiguration<Gateway>
    {
        public GatewayConfiguration()
        {
            HasKey(g => g.Id);
            HasRequired(g => g.Network)
                .WithMany(n => n.Gateways)
                .HasForeignKey(g => g.Network_Id);
            Property(g => g.Address)
                .HasMaxLength(10)
                .IsRequired();
            Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();
            
        }
    }
}