namespace AcademicService.Data.Models;

public class LecturePdf
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; } = default!;
    public Guid LectureId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Lecture Lecture { get; set; } = default!;
}
