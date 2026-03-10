using ExamService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamService.Data.Configurations;

public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("QuizQuestions");

        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Type)
            .HasConversion<string>();

        builder.Property(q => q.Points)
            .HasPrecision(5, 2);

        builder.Property(q => q.CorrectAnswer)
            .HasMaxLength(500);

        builder.Property(q => q.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(q => q.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.Options)
            .WithOne(o => o.QuizQuestion)
            .HasForeignKey(o => o.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
