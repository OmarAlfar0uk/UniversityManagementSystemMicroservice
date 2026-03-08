using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class LectureVideoConfiguration : IEntityTypeConfiguration<LectureVideo>
{
    public void Configure(EntityTypeBuilder<LectureVideo> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("LectureVideos");

        builder.Property(v => v.VideoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(v => v.Lecture)
            .WithOne(l => l.Video)
            .HasForeignKey<LectureVideo>(v => v.LectureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
