namespace AcademicService.Features.Assignments.GetAssignmentStats;

public record AssignmentStatsResponse(
    int TotalSubmissions,
    bool IsOpen,
    DateTime Deadline
);
