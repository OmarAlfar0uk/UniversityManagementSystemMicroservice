using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademicService.Data.Configurations;

public class LecturePdfConfiguration : IEntityTypeConfiguration<LecturePdf>
{
    public void Configure(EntityTypeBuilder<LecturePdf> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("LecturePdfs");

        builder.Property(p => p.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.Lecture)
            .WithOne(l => l.Pdf)
            .HasForeignKey<LecturePdf>(p => p.LectureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
