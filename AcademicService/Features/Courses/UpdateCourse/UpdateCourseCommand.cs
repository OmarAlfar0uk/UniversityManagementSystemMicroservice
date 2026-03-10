using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.Courses.UpdateCourse;

public record UpdateCourseCommand(
    Guid CourseId,
    string Name,
    string? Description,
    IFormFile? CoverImage,
    Guid DoctorId
) : IRequest<CourseResponse>;
