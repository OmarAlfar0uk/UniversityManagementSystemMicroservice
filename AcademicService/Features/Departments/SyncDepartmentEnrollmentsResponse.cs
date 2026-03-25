namespace AcademicService.Features.Departments;

public record SyncDepartmentEnrollmentsResponse(
    Guid DepartmentId,
    int StudentCount,
    int CourseOfferingCount,
    int CreatedEnrollmentCount
);
