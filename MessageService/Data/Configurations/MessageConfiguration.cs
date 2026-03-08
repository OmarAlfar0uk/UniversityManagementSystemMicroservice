using MessageService.Data.Enums;
using MessageService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageService.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Messages");

        builder.Property(m => m.Content)
            .HasMaxLength(2000);

        builder.Property(m => m.FileUrl)
            .HasMaxLength(500);

        builder.Property(m => m.FileType)
            .HasConversion<string>()
            .HasDefaultValue(FileType.None);

        builder.Property(m => m.IsRead)
            .HasDefaultValue(false);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
