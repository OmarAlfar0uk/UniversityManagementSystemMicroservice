namespace AcademicService.Features.Courses.GetCourseById;

public record CourseDetailsResponse(
    Guid Id,
    string Name,
    string Description,
    string CoverImageUrl,
    Guid DoctorId,
    int TotalLectures,
    decimal CompletionPercentage
);
