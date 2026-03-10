using ExamService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamService.Data.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Quizzes");

        builder.Property(q => q.MaxAttempts)
            .HasDefaultValue(1);

        builder.Property(q => q.IsActive)
            .HasDefaultValue(true);

        builder.Property(q => q.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(q => q.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(q => q.Questions)
            .WithOne(qq => qq.Quiz)
            .HasForeignKey(qq => qq.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Attempts)
            .WithOne(a => a.Quiz)
            .HasForeignKey(a => a.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
