using MediatR;

namespace AcademicService.Features.Courses.DeleteCourse;

public record DeleteCourseCommand(Guid CourseId) : IRequest;
