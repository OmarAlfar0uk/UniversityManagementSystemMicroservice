namespace AcademicService.Features.Courses.GetCourseById;

public record CourseDetailsResponse(
    Guid Id,
    string Name,
    string Description,
    string CoverImageUrl,
    Guid DoctorId,
    string DoctorFirstName,
    string DoctorFullName,
    int TotalLectures,
    decimal CompletionPercentage,
    Guid? DepartmentId = null,
    Guid? CourseCatalogId = null
);
