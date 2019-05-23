using SensorsManager.DomainClasses;
using System.Data.Entity.ModelConfiguration;

namespace SensorsManager.DataLayer
{
    internal class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasKey(u => u.Id);
            Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();
            Property(u => u.Password).IsRequired();
            Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired();
            Property(u => u.CompanyName)
                .HasMaxLength(100)
                .IsOptional();
            Property(u => u.Country)
                .HasMaxLength(50)
                .IsRequired();
            Property(u => u.PhoneNumber).IsRequired();
        }
    }
}