using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.GetLectureById;

public class GetLectureByIdHandler : IRequestHandler<GetLectureByIdQuery, LectureDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLectureByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LectureDetailsResponse> Handle(
        GetLectureByIdQuery request,
        CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null || lecture.CourseId != request.CourseId)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found in course {request.CourseId}.");

        var hasPdf = await _unitOfWork.LecturePdfs.AnyAsync(p => p.LectureId == request.LectureId);
        var hasVideo = await _unitOfWork.LectureVideos.AnyAsync(v => v.LectureId == request.LectureId);
        var hasAssignment = await _unitOfWork.Assignments.AnyAsync(a => a.LectureId == request.LectureId);

        return new LectureDetailsResponse(
            lecture.Id,
            lecture.Title,
            lecture.ThumbnailUrl ?? string.Empty,
            hasPdf,
            hasVideo,
            hasAssignment
        );
    }
}
