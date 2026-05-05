using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.LectureMaterials.GetLectureVideo;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AcademicService.Features.LectureMaterials.UploadLectureVideo;

public class UploadLectureVideoHandler : IRequestHandler<UploadLectureVideoCommand, VideoResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVideoHelper _videoHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadLectureVideoHandler(IUnitOfWork unitOfWork, IVideoHelper videoHelper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _videoHelper = videoHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<VideoResponse> Handle(UploadLectureVideoCommand request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            throw new UnauthorizedAccessException("User not authenticated.");

        var doctorId = Guid.Parse(user.FindFirst("id")?.Value ?? throw new InvalidOperationException("Doctor ID claim not found."));

        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        var course = await _unitOfWork.Courses.GetByIdAsync(lecture.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {lecture.CourseId} not found.");

        if (course.DoctorId != doctorId)
            throw new UnauthorizedAccessException("You do not own this lecture.");

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
