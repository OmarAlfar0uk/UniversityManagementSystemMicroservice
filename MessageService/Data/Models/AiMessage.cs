using MessageService.Data.Enums;

namespace MessageService.Data.Models;

public class AiMessage
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public AiRole Role { get; set; }
    public string Content { get; set; } = default!;
    public string? FileUrl { get; set; }
    public FileType FileType { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
