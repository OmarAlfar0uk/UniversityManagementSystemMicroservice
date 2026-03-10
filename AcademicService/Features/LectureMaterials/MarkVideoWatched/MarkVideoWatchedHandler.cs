using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.MarkVideoWatched;

/// <summary>
/// Signals that the student watched this lecture's video.
/// The actual progress tracking is done by the ProgressService.
/// This handler is a no-op hook — the real update happens via an event/REST call to ProgressService.
/// </summary>
public class MarkVideoWatchedHandler : IRequestHandler<MarkVideoWatchedCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkVideoWatchedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarkVideoWatchedCommand request, CancellationToken cancellationToken)
    {
        // Verify lecture exists
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        // Progress tracking is done by ProgressService — this endpoint just validates and returns 204
        await Task.CompletedTask;
    }
}
