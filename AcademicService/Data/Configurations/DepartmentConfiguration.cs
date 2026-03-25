using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Departments");

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.Property(d => d.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
