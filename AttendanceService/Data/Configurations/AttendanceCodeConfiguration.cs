using AttendanceService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceService.Data.Configurations;

public class AttendanceCodeConfiguration : IEntityTypeConfiguration<AttendanceCode>
{
    public void Configure(EntityTypeBuilder<AttendanceCode> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("AttendanceCodes");

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(a => a.Code)
            .IsUnique();

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(a => a.Records)
            .WithOne(r => r.AttendanceCode)
            .HasForeignKey(r => r.AttendanceCodeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
