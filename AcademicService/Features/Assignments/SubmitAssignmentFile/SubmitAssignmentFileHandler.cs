using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Middlewares;
using MassTransit;
using MediatR;
using Shered.Events;


namespace AcademicService.Features.Assignments.SubmitAssignmentFile;

public class SubmitAssignmentFileHandler : IRequestHandler<SubmitAssignmentFileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitAssignmentFileHandler(IUnitOfWork unitOfWork, IFileHelper fileHelper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(SubmitAssignmentFileCommand request, CancellationToken cancellationToken)
    {
        var assignment = request.ByAssignmentId
            ? await _unitOfWork.Assignments.GetByIdAsync(request.AssignmentOrLectureId)
            : await _unitOfWork.Assignments.FindAsync(a => a.LectureId == request.AssignmentOrLectureId);

        if (assignment is null)
            throw new KeyNotFoundException(
                request.ByAssignmentId
                    ? $"Assignment {request.AssignmentOrLectureId} not found."
                    : $"No assignment found for lecture {request.AssignmentOrLectureId}.");

        if (!assignment.IsOpen)
            throw new ArgumentException("Assignment is closed.");

        if (DateTime.UtcNow > assignment.Deadline)
            throw new ArgumentException("Deadline has passed");

        var existing = await _unitOfWork.AssignmentSubmissions.FindAsync(
            s => s.AssignmentId == assignment.Id && s.StudentId == request.StudentId);
        if (existing is not null)
            throw new ConflictException("Assignment already submitted.");

        var relativePath = await _fileHelper.SaveFileAsync(request.File, "Assignments");
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
            FileUrl = relativePath,
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
