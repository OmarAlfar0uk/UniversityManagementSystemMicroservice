using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.Courses.CreateCourse;

public record CreateCourseCommand(
    string Name,
    string? Description,
    IFormFile? CoverImage,
    Guid DoctorId
) : IRequest<CourseResponse>;
