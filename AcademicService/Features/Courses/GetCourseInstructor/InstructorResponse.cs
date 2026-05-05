namespace AcademicService.Features.Courses.GetCourseInstructor;

public record InstructorResponse(
    Guid DoctorId,
    Guid CourseId,
    string FirstName,
    string FullName,
    string? ProfileImage,
    string? Department
);
