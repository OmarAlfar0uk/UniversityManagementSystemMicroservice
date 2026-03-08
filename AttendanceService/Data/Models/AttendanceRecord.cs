namespace AttendanceService.Data.Models;

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime RegisteredAt { get; set; }
    public bool IsManual { get; set; }
    public Guid? AttendanceCodeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public AttendanceCode? AttendanceCode { get; set; }
}
