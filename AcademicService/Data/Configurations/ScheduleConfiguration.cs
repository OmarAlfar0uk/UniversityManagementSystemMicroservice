using AcademicService.Data.Enums;
using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Schedules");

        builder.Property(s => s.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Type)
            .HasConversion<string>();

        builder.Property(s => s.AcademicYear)
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
