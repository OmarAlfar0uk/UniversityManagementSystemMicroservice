using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignmentStatus;

public record GetAssignmentStatusQuery(Guid LectureId, Guid StudentId) : IRequest<AssignmentStatusResponse>;
