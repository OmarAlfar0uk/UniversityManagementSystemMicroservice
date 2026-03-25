using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Lectures");

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(l => l.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(l => l.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(l => l.Course)
            .WithMany(c => c.Lectures)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Pdf)
            .WithOne(p => p.Lecture)
            .HasForeignKey<LecturePdf>(p => p.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Video)
            .WithOne(v => v.Lecture)
            .HasForeignKey<LectureVideo>(v => v.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Assignments)
            .WithOne(a => a.Lecture)
            .HasForeignKey(a => a.LectureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
