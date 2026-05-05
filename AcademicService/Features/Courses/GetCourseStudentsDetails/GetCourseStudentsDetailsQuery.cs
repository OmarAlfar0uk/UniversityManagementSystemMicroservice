using MediatR;

namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public record GetCourseStudentsDetailsQuery(Guid CourseId) : IRequest<List<StudentDetailItem>>;
