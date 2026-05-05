using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.GetAssignment;

public class GetAssignmentHandler : IRequestHandler<GetAssignmentQuery, AssignmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;

    public GetAssignmentHandler(IUnitOfWork unitOfWork, IFileHelper fileHelper)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
    }

    public async Task<AssignmentResponse> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.LectureId);
        if (assignment is null)
            throw new KeyNotFoundException($"No assignment found for lecture {request.LectureId}.");

        return new AssignmentResponse(
            assignment.Id,
            assignment.Title,
            string.IsNullOrEmpty(assignment.FileUrl) ? string.Empty : _fileHelper.GetFileUrl(assignment.FileUrl),
            assignment.LectureId
        );
    }
}
