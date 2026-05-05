namespace AcademicService.Features.Courses.GetCourseInstructor;

public record InstructorResponse(
    Guid DoctorId,
    Guid CourseId,
    string FullName,
    string? ProfileImage,
    string? Department
);
