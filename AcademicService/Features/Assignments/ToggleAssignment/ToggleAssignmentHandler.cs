using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Assignments.ToggleAssignment;

public class ToggleAssignmentHandler : IRequestHandler<ToggleAssignmentCommand, ToggleAssignmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleAssignmentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ToggleAssignmentResponse> Handle(ToggleAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.AssignmentId);
        if (assignment is null)
            throw new KeyNotFoundException($"Assignment {request.AssignmentId} not found.");

        assignment.IsOpen = !assignment.IsOpen;
        assignment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Assignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync();

        return new ToggleAssignmentResponse(assignment.Id, assignment.IsOpen);
    }
}
