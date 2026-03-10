using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetGPA;

public record GetGPAQuery(Guid StudentId) : IRequest<GPAResponse>;
