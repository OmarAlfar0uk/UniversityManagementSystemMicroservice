using ExamService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamService.Data.Configurations;

public class QuizQuestionOptionConfiguration : IEntityTypeConfiguration<QuizQuestionOption>
{
    public void Configure(EntityTypeBuilder<QuizQuestionOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("QuizQuestionOptions");

        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.IsCorrect)
            .HasDefaultValue(false);

        builder.Property(o => o.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(o => o.QuizQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuizQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
