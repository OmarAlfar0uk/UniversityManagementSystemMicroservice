using AcademicService.Data.Enums;

namespace AcademicService.Data.Models;

public class Schedule
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = default!;
    public ScheduleType Type { get; set; }
    public string? AcademicYear { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
