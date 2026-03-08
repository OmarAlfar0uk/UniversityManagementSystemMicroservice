namespace AcademicService.Data.Models;

public class Assignment
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? FileUrl { get; set; }
    public Guid LectureId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Lecture Lecture { get; set; } = default!;
    public ICollection<AssignmentSubmission> Submissions { get; set; } = [];
}
