using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLectureVideo;

public class GetLectureVideoHandler : IRequestHandler<GetLectureVideoQuery, VideoResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLectureVideoHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VideoResponse> Handle(GetLectureVideoQuery request, CancellationToken cancellationToken)
    {
        var video = await _unitOfWork.LectureVideos.FindAsync(v => v.LectureId == request.LectureId);
        if (video is null)
            throw new KeyNotFoundException($"No video found for lecture {request.LectureId}.");

        return new VideoResponse(video.Id, video.VideoUrl, video.DurationInMinutes, video.LectureId);
    }
}
