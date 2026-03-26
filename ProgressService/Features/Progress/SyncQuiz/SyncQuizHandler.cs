using MediatR;
using Microsoft.AspNetCore.Http;
using ProgressService.Contracts;
using ProgressService.Data.Models;

namespace ProgressService.Features.Progress.SyncQuiz;

public class SyncQuizHandler(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<SyncQuizCommand, ProgressResponse>
{
    public async Task<ProgressResponse> Handle(SyncQuizCommand request, CancellationToken ct)
    {
        if (!httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("X-Internal-Request"))
            throw new UnauthorizedAccessException("Internal access only.");

        var progress = await uow.LectureProgresses.FindAsync(
            p => p.LectureId == request.LectureId && p.StudentId == request.StudentId);

        if (progress is null)
        {
            progress = new LectureProgress
            {
                Id              = Guid.NewGuid(),
                StudentId       = request.StudentId,
                LectureId       = request.LectureId,
                CourseId        = request.CourseId,
                IsQuizCompleted = true,
                CreatedAt       = DateTime.UtcNow,
                UpdatedAt       = DateTime.UtcNow
            };
            progress.CompletionPercentage = CalculateCompletion(progress);
            await uow.LectureProgresses.AddAsync(progress);
        }
        else
        {
            progress.IsQuizCompleted = true;
            progress.CompletionPercentage = CalculateCompletion(progress);
            progress.UpdatedAt = DateTime.UtcNow;
            uow.LectureProgresses.Update(progress);
        }

        await uow.SaveChangesAsync();
        return ToResponse(progress);
    }

    private static decimal CalculateCompletion(LectureProgress p) =>
        ((p.IsPdfViewed              ? 1 : 0) +
         (p.IsVideoWatched           ? 1 : 0) +
         (p.IsAttendanceRegistered   ? 1 : 0) +
         (p.IsAssignmentSubmitted    ? 1 : 0) +
         (p.IsQuizCompleted          ? 1 : 0)) * 20m;

    private static ProgressResponse ToResponse(LectureProgress p) =>
        new(p.Id, p.StudentId, p.LectureId, p.CourseId,
            p.IsPdfViewed, p.IsVideoWatched, p.IsAttendanceRegistered,
            p.IsAssignmentSubmitted, p.IsQuizCompleted,
            p.CompletionPercentage, p.UpdatedAt);
}
