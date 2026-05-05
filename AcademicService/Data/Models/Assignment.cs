namespace AcademicService.Data.Models;

public class Assignment
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Instructions { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsOpen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Lecture Lecture { get; set; } = default!;
    public ICollection<AssignmentSubmission> Submissions { get; set; } = [];
}
