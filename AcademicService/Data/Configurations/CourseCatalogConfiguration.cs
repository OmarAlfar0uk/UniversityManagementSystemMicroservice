using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class CourseCatalogConfiguration : IEntityTypeConfiguration<CourseCatalog>
{
    public void Configure(EntityTypeBuilder<CourseCatalog> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("CourseCatalogs");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.CoverImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
