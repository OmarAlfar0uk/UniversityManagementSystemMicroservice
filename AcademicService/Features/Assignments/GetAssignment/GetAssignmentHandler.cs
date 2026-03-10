using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignment;

public class GetAssignmentHandler : IRequestHandler<GetAssignmentQuery, AssignmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAssignmentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssignmentResponse> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.LectureId);
        if (assignment is null)
            throw new KeyNotFoundException($"No assignment found for lecture {request.LectureId}.");

        return new AssignmentResponse(assignment.Id, assignment.Title, assignment.FileUrl ?? string.Empty, assignment.LectureId);
    }
}
