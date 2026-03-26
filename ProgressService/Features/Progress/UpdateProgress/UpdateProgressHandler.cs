using ProgressService.Contracts;
using ProgressService.Data.Models;
using MediatR;

namespace ProgressService.Features.Progress.UpdateProgress;

public class UpdateProgressHandler : IRequestHandler<UpdateProgressCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProgressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProgressCommand request, CancellationToken cancellationToken)
    {
        // 1. Upsert lecture progress
        var lectureProgress = await _unitOfWork.LectureProgresses.FindAsync(
            p => p.LectureId == request.LectureId && p.StudentId == request.StudentId);

        if (lectureProgress is null)
        {
            lectureProgress = new LectureProgress
            {
                Id = Guid.NewGuid(),
                LectureId = request.LectureId,
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                CompletionPercentage = 100m,
                IsVideoWatched = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.LectureProgresses.AddAsync(lectureProgress);
        }
        else if (lectureProgress.CompletionPercentage < 100m)
        {
            lectureProgress.CompletionPercentage = 100m;
            lectureProgress.IsVideoWatched = true;
            lectureProgress.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.LectureProgresses.Update(lectureProgress);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
