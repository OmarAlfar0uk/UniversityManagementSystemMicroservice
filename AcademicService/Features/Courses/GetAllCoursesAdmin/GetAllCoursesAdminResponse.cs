namespace AcademicService.Features.Courses.GetAllCoursesAdmin;

public record GetAllCoursesAdminResponse(List<AdminCourseItem> Courses);

public record AdminCourseItem(
    Guid Id,
    string Name,
    Guid DoctorId,
    string? CoverImageUrl,
    int EnrolledCount,
    Guid? DepartmentId
);
