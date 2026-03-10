using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.LectureMaterials.GetLectureVideo;
using MediatR;

namespace AcademicService.Features.LectureMaterials.UploadLectureVideo;

public class UploadLectureVideoHandler : IRequestHandler<UploadLectureVideoCommand, VideoResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVideoHelper _videoHelper;

    public UploadLectureVideoHandler(IUnitOfWork unitOfWork, IVideoHelper videoHelper)
    {
        _unitOfWork = unitOfWork;
        _videoHelper = videoHelper;
    }

    public async Task<VideoResponse> Handle(UploadLectureVideoCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        // Remove existing video file and record if any
        var existing = await _unitOfWork.LectureVideos.FindAsync(v => v.LectureId == request.LectureId);
        if (existing is not null)
        {
            _videoHelper.DeleteVideo(existing.VideoUrl);
            _unitOfWork.LectureVideos.Remove(existing);
        }

        // Save new video file
        var relativePath = await _videoHelper.SaveVideoAsync(request.VideoFile, "Lectures");

        // Read duration from saved file metadata
        var duration = await _videoHelper.GetVideoDurationInMinutesAsync(relativePath);

        var video = new LectureVideo
        {
            Id = Guid.NewGuid(),
            LectureId = request.LectureId,
            VideoUrl = relativePath,
            DurationInMinutes = duration,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.LectureVideos.AddAsync(video);
        await _unitOfWork.SaveChangesAsync();

        return new VideoResponse(video.Id, _videoHelper.GetVideoUrl(video.VideoUrl), video.DurationInMinutes, video.LectureId);
    }
}
