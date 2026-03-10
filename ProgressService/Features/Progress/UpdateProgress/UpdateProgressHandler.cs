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
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.LectureProgresses.AddAsync(lectureProgress);
        }
        else if (!lectureProgress.IsCompleted)
        {
            lectureProgress.IsCompleted = true;
            lectureProgress.CompletedAt = DateTime.UtcNow;
            lectureProgress.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.LectureProgresses.Update(lectureProgress);
        }

        // 2. Recalculate course progress
        var allLectureProgresses = await _unitOfWork.LectureProgresses.GetAllAsync(
            p => p.CourseId == request.CourseId && p.StudentId == request.StudentId);

        var completed = allLectureProgresses.Count(p => p.IsCompleted);
        var percentage = request.TotalLecturesInCourse > 0
            ? Math.Round((decimal)completed / request.TotalLecturesInCourse * 100, 2)
            : 0m;

        var courseProgress = await _unitOfWork.CourseProgresses.FindAsync(
            p => p.CourseId == request.CourseId && p.StudentId == request.StudentId);

        if (courseProgress is null)
        {
            courseProgress = new CourseProgress
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                CompletedLectures = completed,
                CompletionPercentage = percentage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.CourseProgresses.AddAsync(courseProgress);
        }
        else
        {
            courseProgress.CompletedLectures = completed;
            courseProgress.CompletionPercentage = percentage;
            courseProgress.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CourseProgresses.Update(courseProgress);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
