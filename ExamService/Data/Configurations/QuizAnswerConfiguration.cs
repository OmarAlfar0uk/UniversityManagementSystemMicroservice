using ExamService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamService.Data.Configurations;

public class QuizAnswerConfiguration : IEntityTypeConfiguration<QuizAnswer>
{
    public void Configure(EntityTypeBuilder<QuizAnswer> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("QuizAnswers");

        builder.Property(a => a.AnswerText)
            .HasMaxLength(2000);

        builder.Property(a => a.EarnedPoints)
            .HasPrecision(5, 2);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(a => a.QuizAttempt)
            .WithMany(at => at.Answers)
            .HasForeignKey(a => a.QuizAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.QuizQuestion)
            .WithMany()
            .HasForeignKey(a => a.QuizQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
