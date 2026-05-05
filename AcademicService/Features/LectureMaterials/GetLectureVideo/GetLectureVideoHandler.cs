using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLectureVideo;

public class GetLectureVideoHandler : IRequestHandler<GetLectureVideoQuery, VideoResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVideoHelper _videoHelper;

    public GetLectureVideoHandler(IUnitOfWork unitOfWork, IVideoHelper videoHelper)
    {
        _unitOfWork = unitOfWork;
        _videoHelper = videoHelper;
    }

    public async Task<VideoResponse> Handle(GetLectureVideoQuery request, CancellationToken cancellationToken)
    {
        var video = await _unitOfWork.LectureVideos.FindAsync(v => v.LectureId == request.LectureId);
        if (video is null)
            throw new KeyNotFoundException($"No video found for lecture {request.LectureId}.");

        return new VideoResponse(
            video.Id,
            string.IsNullOrEmpty(video.VideoUrl) ? string.Empty : _videoHelper.GetVideoUrl(video.VideoUrl),
            video.DurationInMinutes,
            video.LectureId
        );
    }
}
