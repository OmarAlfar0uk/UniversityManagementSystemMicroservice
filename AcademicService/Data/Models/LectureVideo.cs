namespace AcademicService.Data.Models;

public class LectureVideo
{
    public Guid Id { get; set; }
    public string VideoUrl { get; set; } = default!;
    public int DurationInMinutes { get; set; }
    public Guid LectureId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Lecture Lecture { get; set; } = default!;
}
