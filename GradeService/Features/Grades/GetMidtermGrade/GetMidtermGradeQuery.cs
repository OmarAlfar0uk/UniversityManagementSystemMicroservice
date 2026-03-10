using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetMidtermGrade;

public record GetMidtermGradeQuery(Guid CourseId, Guid StudentId) : IRequest<MidtermGradeResponse>;
