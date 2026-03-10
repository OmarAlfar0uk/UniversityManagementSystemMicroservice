using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressService.Data.Models;

namespace ProgressService.Data.Configurations;

public class LectureProgressConfiguration : IEntityTypeConfiguration<LectureProgress>
{
    public void Configure(EntityTypeBuilder<LectureProgress> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("LectureProgresses");

        builder.Property(p => p.IsPdfViewed)
            .HasDefaultValue(false);

        builder.Property(p => p.IsVideoWatched)
            .HasDefaultValue(false);

        builder.Property(p => p.IsAttendanceRegistered)
            .HasDefaultValue(false);

        builder.Property(p => p.IsAssignmentSubmitted)
            .HasDefaultValue(false);

        builder.Property(p => p.IsQuizCompleted)
            .HasDefaultValue(false);

        builder.Property(p => p.CompletionPercentage)
            .HasPrecision(5, 2);

        builder.HasIndex(p => new { p.StudentId, p.LectureId })
            .IsUnique();

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
