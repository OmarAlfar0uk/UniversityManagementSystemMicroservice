namespace AuthService.Features.Parent.GetChildSchedule
{
    public record ChildScheduleResponse(
        Guid               StudentId,
        string             StudentName,
        Guid?              DepartmentId,
        List<ScheduleItem> Schedules
    );

    public record ScheduleItem(
        Guid    Id,
        string  ImageUrl,
        string  Type,           // "ClassSchedule" or "MidtermExamSchedule"
        string? AcademicYear
    );
}
