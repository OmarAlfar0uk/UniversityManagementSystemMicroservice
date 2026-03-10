using MessageService.Data.Enums;
using MessageService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageService.Data.Configurations;

public class AiMessageConfiguration : IEntityTypeConfiguration<AiMessage>
{
    public void Configure(EntityTypeBuilder<AiMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("AiMessages");

        builder.Property(m => m.Role)
            .HasConversion<string>();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(m => m.FileUrl)
            .HasMaxLength(500);

        builder.Property(m => m.FileType)
            .HasConversion<string>()
            .HasDefaultValue(FileType.None);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
