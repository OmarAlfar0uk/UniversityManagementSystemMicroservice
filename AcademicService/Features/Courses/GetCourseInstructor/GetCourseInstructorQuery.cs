using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseInstructor;

public record GetCourseInstructorQuery(Guid CourseId) : IRequest<InstructorResponse>;
