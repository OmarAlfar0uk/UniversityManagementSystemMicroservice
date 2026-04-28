namespace AcademicService.Features.Schedule.GetClassSchedule;

public record ScheduleResponse(
    Guid Id,
    string ImageUrl,
    string Type,
    string? AcademicYear,
    Guid DepartmentId
);
