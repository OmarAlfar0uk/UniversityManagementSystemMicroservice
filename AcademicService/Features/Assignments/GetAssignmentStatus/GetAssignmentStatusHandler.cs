using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignmentStatus;

public class GetAssignmentStatusHandler : IRequestHandler<GetAssignmentStatusQuery, AssignmentStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAssignmentStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignmentStatusResponse> Handle(GetAssignmentStatusQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.LectureId);
        if (assignment is null)
            throw new KeyNotFoundException($"No assignment found for lecture {request.LectureId}.");

        var submission = await _unitOfWork.AssignmentSubmissions.FindAsync(
            s => s.AssignmentId == assignment.Id && s.StudentId == request.StudentId);

        return new AssignmentStatusResponse(
            IsSubmitted: submission is not null,
            SubmittedAt: submission?.SubmittedAt
        );
    }
}
