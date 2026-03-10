using Auth.Models;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Data.Configurations
{
    public class ActivationCodeConfig : IEntityTypeConfiguration<ActivationCode>
    {
        public void Configure(EntityTypeBuilder<ActivationCode> builder)
        {
            builder.ToTable("ActivationCodes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                   .IsUnique();

            builder.Property(x => x.Role)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.ExpiryDate)
                   .IsRequired();

            builder.Property(x => x.IsUsed)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
