namespace NotificationService.Data.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public Guid? SenderId { get; set; }
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public bool IsRead { get; set; }
    public string? SenderImageUrl { get; set; }
    public string? MaterialName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
