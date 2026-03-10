using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseById;

public record GetCourseByIdQuery(
    Guid CourseId,
    Guid StudentId
) : IRequest<CourseDetailsResponse>;
