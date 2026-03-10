using GradeService.Contracts;
using GradeService.Features.Grades.GetMidtermGrade;
using MediatR;

namespace GradeService.Features.Grades.SetMidtermGrade;

public record SetMidtermGradeCommand(Guid CourseId, Guid StudentId, decimal Score) : IRequest<MidtermGradeResponse>;
