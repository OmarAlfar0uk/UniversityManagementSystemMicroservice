using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetMidtermGrade;

public class GetMidtermGradeHandler : IRequestHandler<GetMidtermGradeQuery, MidtermGradeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMidtermGradeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MidtermGradeResponse> Handle(GetMidtermGradeQuery request, CancellationToken cancellationToken)
    {
        var grade = await _unitOfWork.StudentGrades.FindAsync(
            g => g.CourseId == request.CourseId && g.StudentId == request.StudentId);

        if (grade is null)
            return new MidtermGradeResponse(request.CourseId, request.StudentId, null, null);

        return new MidtermGradeResponse(grade.CourseId, grade.StudentId, grade.MidtermScore, CalculateLetter(grade.MidtermScore));
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
}
