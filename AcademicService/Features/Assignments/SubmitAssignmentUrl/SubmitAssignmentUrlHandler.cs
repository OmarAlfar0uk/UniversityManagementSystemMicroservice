using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Middlewares;
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

    public async Task<Unit> Handle(SubmitAssignmentUrlCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.LectureId);
        if (assignment is null)
            throw new KeyNotFoundException($"No assignment found for lecture {request.LectureId}.");

        if (!assignment.IsOpen)
            throw new ArgumentException("Assignment is closed.");

        if (DateTime.UtcNow > assignment.Deadline)
            throw new ArgumentException("Deadline has passed");

        var existing = await _unitOfWork.AssignmentSubmissions.FindAsync(
            s => s.AssignmentId == assignment.Id && s.StudentId == request.StudentId);
        if (existing is not null)
            throw new ConflictException("Assignment already submitted.");

        var enrollment = await _unitOfWork.CourseEnrollments.FindAsync(
            e => e.CourseId == assignment.CourseId && e.StudentId == request.StudentId);

        var submission = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = request.StudentId,
            StudentFirstName = enrollment?.StudentFirstName ?? string.Empty,
            StudentFullName = enrollment?.StudentFullName ?? string.Empty,
            StudentEmail = enrollment?.StudentEmail ?? string.Empty,
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

        return Unit.Value;
    }
}
