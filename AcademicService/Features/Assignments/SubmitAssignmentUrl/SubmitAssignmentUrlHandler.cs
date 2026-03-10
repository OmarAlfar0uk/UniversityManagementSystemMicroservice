using AcademicService.Contracts;
using AcademicService.Data.Models;
using MassTransit;
using MediatR;
using Shered.Events;

namespace AcademicService.Features.Assignments.SubmitAssignmentUrl;

public class SubmitAssignmentUrlHandler : IRequestHandler<SubmitAssignmentUrlCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitAssignmentUrlHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(SubmitAssignmentUrlCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.LectureId);
        if (assignment is null)
            throw new KeyNotFoundException($"No assignment found for lecture {request.LectureId}.");

        var existing = await _unitOfWork.AssignmentSubmissions.FindAsync(
            s => s.AssignmentId == assignment.Id && s.StudentId == request.StudentId);
        if (existing is not null)
            throw new InvalidOperationException("Assignment already submitted.");

        var submission = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = request.StudentId,
            ProjectUrl = request.ProjectUrl,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AssignmentSubmissions.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        // ✅ Publish event to RabbitMQ
        await _publishEndpoint.Publish<IAssignmentSubmitted>(new
        {
            StudentId = submission.StudentId,
            AssignmentId = submission.AssignmentId,
            SubmissionDate = submission.SubmittedAt
        }, cancellationToken);
    }
}
