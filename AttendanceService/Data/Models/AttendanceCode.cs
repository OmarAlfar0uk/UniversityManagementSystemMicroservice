namespace AttendanceService.Data.Models;

public class AttendanceCode
{
    public Guid Id { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
    public string Code { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<AttendanceRecord> Records { get; set; } = [];
}
