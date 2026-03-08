namespace MessageService.Data.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
}
