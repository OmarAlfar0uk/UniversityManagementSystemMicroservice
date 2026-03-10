using MediatR;

namespace AcademicService.Features.Courses.EnrollStudents;

public record EnrollStudentsCommand(
    Guid CourseId,
    List<Guid> StudentIds
) : IRequest;
