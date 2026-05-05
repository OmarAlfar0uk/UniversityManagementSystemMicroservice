namespace AcademicService.Features.Courses.GetAllCourses;

public record CourseResponse(
    Guid Id,
    string Name,
    string Description,
    string CoverImageUrl,
    Guid DoctorId,
    string DoctorFirstName,
    string DoctorFullName,
    decimal CompletionPercentage,
    Guid? DepartmentId = null,
    Guid? CourseCatalogId = null
);
