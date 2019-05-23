using SensorsManager.DomainClasses;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class NetworkConfiguration : EntityTypeConfiguration<Network>
    {
        public NetworkConfiguration()
        {
            HasKey(n => n.Id);
            Property(n => n.Address)
            .IsFixedLength()
            .HasMaxLength(10)
            .IsRequired();
            HasRequired(n => n.User)
                .WithMany(u => u.Networks)
                .HasForeignKey(n => n.User_Id);
            Property(n => n.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}