using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignmentStats;

public class GetAssignmentStatsHandler : IRequestHandler<GetAssignmentStatsQuery, AssignmentStatsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAssignmentStatsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignmentStatsResponse> Handle(GetAssignmentStatsQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.AssignmentId);
        if (assignment is null)
            throw new KeyNotFoundException($"Assignment {request.AssignmentId} not found.");

        var totalSubmissions = await _unitOfWork.AssignmentSubmissions
            .CountAsync(s => s.AssignmentId == request.AssignmentId);

        return new AssignmentStatsResponse(totalSubmissions, assignment.IsOpen, assignment.Deadline);
    }
}
