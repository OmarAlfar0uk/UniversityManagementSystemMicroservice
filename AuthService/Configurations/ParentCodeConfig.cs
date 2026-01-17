using Auth.Models;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.Configurations
{
    public class ParentCodeConfig : IEntityTypeConfiguration<ParentCode>
    {
        public void Configure(EntityTypeBuilder<ParentCode> builder)
        {
            builder.ToTable("ParentCodes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                   .IsUnique();

            builder.Property(x => x.ExpiryDate)
                   .IsRequired();

            builder.Property(x => x.IsUsed)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            // Relation مع Student
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(x => x.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
