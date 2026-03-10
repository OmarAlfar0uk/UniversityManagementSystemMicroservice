using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignment;

public record GetAssignmentQuery(Guid LectureId) : IRequest<AssignmentResponse>;
