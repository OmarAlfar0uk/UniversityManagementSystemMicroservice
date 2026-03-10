using GradeService.Contracts;
using GradeService.Data.Models;
using GradeService.Features.Grades.GetMidtermGrade;
using MediatR;

namespace GradeService.Features.Grades.SetMidtermGrade;

public class SetMidtermGradeHandler : IRequestHandler<SetMidtermGradeCommand, MidtermGradeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMidtermGradeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MidtermGradeResponse> Handle(SetMidtermGradeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.StudentGrades.FindAsync(
            g => g.CourseId == request.CourseId && g.StudentId == request.StudentId);

        var letter = CalculateLetter(request.Score);

        if (existing is not null)
        {
            existing.MidtermScore = request.Score;
            existing.TotalScore = existing.AttendanceScore + existing.AssignmentScore + existing.QuizScore + existing.MidtermScore + existing.FinalScore;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.StudentGrades.Update(existing);
        }
        else
        {
            var grade = new StudentGrade
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                MidtermScore = request.Score,
                TotalScore = request.Score,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.StudentGrades.AddAsync(grade);
        }

        await _unitOfWork.SaveChangesAsync();
        return new MidtermGradeResponse(request.CourseId, request.StudentId, request.Score, letter);
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
