namespace AcademicService.Data.Models;

public class Lecture
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int OrderIndex { get; set; }
    public string? ThumbnailUrl { get; set; }
    public Guid CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Course Course { get; set; } = default!;
    public LecturePdf? Pdf { get; set; }
    public LectureVideo? Video { get; set; }
    public ICollection<Assignment> Assignments { get; set; } = [];
}
