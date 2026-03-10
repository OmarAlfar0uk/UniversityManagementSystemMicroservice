using AttendanceService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceService.Data.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("AttendanceRecords");

        builder.Property(r => r.IsManual)
            .HasDefaultValue(false);

        builder.HasIndex(r => new { r.StudentId, r.LectureId })
            .IsUnique();

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(r => r.AttendanceCode)
            .WithMany(a => a.Records)
            .HasForeignKey(r => r.AttendanceCodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
