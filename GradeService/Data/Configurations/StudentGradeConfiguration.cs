using GradeService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradeService.Data.Configurations;

public class StudentGradeConfiguration : IEntityTypeConfiguration<StudentGrade>
{
    public void Configure(EntityTypeBuilder<StudentGrade> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("StudentGrades");

        builder.Property(g => g.AttendanceScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.Property(g => g.AssignmentScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.Property(g => g.QuizScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.Property(g => g.MidtermScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.Property(g => g.FinalScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.Property(g => g.TotalScore)
            .HasPrecision(5, 2);

        builder.HasIndex(g => new { g.StudentId, g.CourseId })
            .IsUnique();

        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(g => g.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
