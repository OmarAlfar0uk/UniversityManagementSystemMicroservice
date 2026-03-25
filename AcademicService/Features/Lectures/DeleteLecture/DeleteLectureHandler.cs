using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.DeleteLecture;

public class DeleteLectureHandler : IRequestHandler<DeleteLectureCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;
    private readonly IImageHelper _imageHelper;
    private readonly IVideoHelper _videoHelper;

    public DeleteLectureHandler(
        IUnitOfWork unitOfWork,
        IFileHelper fileHelper,
        IImageHelper imageHelper,
        IVideoHelper videoHelper)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
        _imageHelper = imageHelper;
        _videoHelper = videoHelper;
    }

    public async Task<Unit> Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        await DeleteLectureDependenciesAsync(lecture.Id, lecture.ThumbnailUrl);
        await _unitOfWork.SaveChangesAsync();

        _unitOfWork.Lectures.Remove(lecture);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }

    // Clean child records explicitly so delete keeps working even when the live DB still has restrictive FKs.
    private async Task DeleteLectureDependenciesAsync(Guid lectureId, string? thumbnailUrl)
    {
        if (!string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            _imageHelper.DeleteImage(thumbnailUrl);
        }

        var pdf = await _unitOfWork.LecturePdfs.FindAsync(p => p.LectureId == lectureId);
        if (pdf is not null)
        {
            _fileHelper.DeleteFile(pdf.FileUrl);
            _unitOfWork.LecturePdfs.Remove(pdf);
        }

        var video = await _unitOfWork.LectureVideos.FindAsync(v => v.LectureId == lectureId);
        if (video is not null)
        {
            _videoHelper.DeleteVideo(video.VideoUrl);
            _unitOfWork.LectureVideos.Remove(video);
        }

        var assignments = (await _unitOfWork.Assignments.GetAllAsync(a => a.LectureId == lectureId)).ToList();
        foreach (var assignment in assignments)
        {
            if (!string.IsNullOrWhiteSpace(assignment.FileUrl))
            {
                _fileHelper.DeleteFile(assignment.FileUrl);
            }

            var submissions = (await _unitOfWork.AssignmentSubmissions
                .GetAllAsync(s => s.AssignmentId == assignment.Id))
                .ToList();

            foreach (var submission in submissions)
            {
                if (!string.IsNullOrWhiteSpace(submission.FileUrl))
                {
                    _fileHelper.DeleteFile(submission.FileUrl);
                }
            }

            if (submissions.Count > 0)
            {
                _unitOfWork.AssignmentSubmissions.RemoveRange(submissions);
            }
        }

        if (assignments.Count > 0)
        {
            _unitOfWork.Assignments.RemoveRange(assignments);
        }
    }
}
