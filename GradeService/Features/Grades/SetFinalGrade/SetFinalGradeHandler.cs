using GradeService.Contracts;
using GradeService.Data.Models;
using GradeService.Features.Grades.GetFinalGrade;
using MassTransit;
using MediatR;
using Shered.Events;

namespace GradeService.Features.Grades.SetFinalGrade;

public class SetFinalGradeHandler : IRequestHandler<SetFinalGradeCommand, FinalGradeResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public SetFinalGradeHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<FinalGradeResponse> Handle(SetFinalGradeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.StudentGrades.FindAsync(
            g => g.CourseId == request.CourseId && g.StudentId == request.StudentId);

        var letter = CalculateLetter(request.Score);
        var points = CalculateGradePoints(request.Score);

        if (existing is not null)
        {
            existing.FinalScore = request.Score;
            existing.TotalScore = existing.AttendanceScore + existing.AssignmentScore + existing.QuizScore + existing.MidtermScore + existing.FinalScore;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.StudentGrades.Update(existing);
        }
        else
        {
            var grade = new StudentGrade
            {
                Id        = Guid.NewGuid(),
                CourseId  = request.CourseId,
                StudentId = request.StudentId,
                FinalScore = request.Score,
                TotalScore = request.Score,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.StudentGrades.AddAsync(grade);
        }

        await _unitOfWork.SaveChangesAsync();

        // ✅ Publish event to RabbitMQ
        await _publishEndpoint.Publish<IGradeAdded>(new
        {
            StudentId = request.StudentId,
            CourseId  = request.CourseId,
            GradeType = "Final",
            Score     = request.Score
        }, cancellationToken);

        return new FinalGradeResponse(request.CourseId, request.StudentId, request.Score, letter, points);
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
