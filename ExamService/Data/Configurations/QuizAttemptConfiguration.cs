using ExamService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamService.Data.Configurations;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("QuizAttempts");

        builder.Property(a => a.Score)
            .HasPrecision(5, 2);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(a => a.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(a => a.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Answers)
            .WithOne(ans => ans.QuizAttempt)
            .HasForeignKey(ans => ans.QuizAttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
