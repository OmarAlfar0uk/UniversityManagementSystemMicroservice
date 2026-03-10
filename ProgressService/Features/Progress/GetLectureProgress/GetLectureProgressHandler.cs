using ProgressService.Contracts;
using MediatR;

namespace ProgressService.Features.Progress.GetLectureProgress;

public class GetLectureProgressHandler : IRequestHandler<GetLectureProgressQuery, LectureProgressResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLectureProgressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LectureProgressResponse> Handle(GetLectureProgressQuery request, CancellationToken cancellationToken)
    {
        var progress = await _unitOfWork.LectureProgresses.FindAsync(
            p => p.LectureId == request.LectureId && p.StudentId == request.StudentId);

        return new LectureProgressResponse(
            request.LectureId,
            request.StudentId,
            progress?.IsCompleted ?? false,
            progress?.CompletedAt
        );
    }
}
