namespace AcademicService.Data.Models;

public class AssignmentSubmission
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public Guid AssignmentId { get; set; }
    public string? FileUrl { get; set; }
    public string? ProjectUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Assignment Assignment { get; set; } = default!;
}
