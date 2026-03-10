using GradeService.Contracts;
using MediatR;

namespace GradeService.Features.Grades.GetGPA;

public class GetGPAHandler : IRequestHandler<GetGPAQuery, GPAResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGPAHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GPAResponse> Handle(GetGPAQuery request, CancellationToken cancellationToken)
    {
        var grades = await _unitOfWork.StudentGrades.GetAllAsync(g => g.StudentId == request.StudentId);

        if (!grades.Any())
            return new GPAResponse(request.StudentId, 0m, 0);

        var pointsList = grades.Select(g => CalculateGradePoints(g.TotalScore)).ToList();
        var gpa = pointsList.Any()
            ? Math.Round(pointsList.Average(), 2)
            : 0m;

        return new GPAResponse(request.StudentId, gpa, pointsList.Count);
    }

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
