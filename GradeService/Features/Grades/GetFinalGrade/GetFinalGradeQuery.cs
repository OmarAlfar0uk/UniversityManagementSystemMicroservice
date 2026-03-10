using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetFinalGrade;

public record GetFinalGradeQuery(Guid CourseId, Guid StudentId) : IRequest<FinalGradeResponse>;
