using GradeService.Contracts;
using GradeService.Features.Grades.GetFinalGrade;
using MediatR;

namespace GradeService.Features.Grades.SetFinalGrade;

public record SetFinalGradeCommand(Guid CourseId, Guid StudentId, decimal Score) : IRequest<FinalGradeResponse>;
