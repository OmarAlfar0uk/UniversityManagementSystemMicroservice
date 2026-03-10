using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetFinalGrade;

public class GetFinalGradeHandler : IRequestHandler<GetFinalGradeQuery, FinalGradeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFinalGradeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FinalGradeResponse> Handle(GetFinalGradeQuery request, CancellationToken cancellationToken)
    {
        var grade = await _unitOfWork.StudentGrades.FindAsync(
            g => g.CourseId == request.CourseId && g.StudentId == request.StudentId);

        if (grade is null)
            return new FinalGradeResponse(request.CourseId, request.StudentId, null, null, null);

        var letter = CalculateLetter(grade.FinalScore);
        var points = CalculateGradePoints(grade.FinalScore);

        return new FinalGradeResponse(grade.CourseId, grade.StudentId, grade.FinalScore, letter, points);
    }

    private static string CalculateLetter(decimal score) => score switch
    {
        >= 90 => "A+",
        >= 85 => "A",
        >= 80 => "B+",
        >= 75 => "B",
        >= 70 => "C+",
        >= 65 => "C",
        >= 60 => "D+",
        >= 50 => "D",
        _ => "F"
    };

    private static decimal CalculateGradePoints(decimal score) => score switch
    {
        >= 90 => 4.0m,
        >= 85 => 4.0m,
        >= 80 => 3.5m,
        >= 75 => 3.5m,
        >= 70 => 3.0m,
        >= 65 => 3.0m,
        >= 60 => 2.5m,
        >= 50 => 2.0m,
        _ => 0.0m
    };
}
