using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.DeleteCourse;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;
    private readonly IImageHelper _imageHelper;
    private readonly IVideoHelper _videoHelper;

    public DeleteCourseHandler(
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

    public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        await DeleteCourseDependenciesAsync(course);
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(course.CoverImageUrl))
        {
            _imageHelper.DeleteImage(course.CoverImageUrl);
        }

        _unitOfWork.Courses.Remove(course);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }

    private async Task DeleteCourseDependenciesAsync(Data.Models.Course course)
    {
        var enrollments = (await _unitOfWork.CourseEnrollments
            .GetAllAsync(e => e.CourseId == course.Id))
            .ToList();

        if (enrollments.Count > 0)
        {
            _unitOfWork.CourseEnrollments.RemoveRange(enrollments);
        }

        var lectures = (await _unitOfWork.Lectures
            .GetAllAsync(l => l.CourseId == course.Id))
            .ToList();

        foreach (var lecture in lectures)
        {
            await DeleteLectureDependenciesAsync(lecture.Id, lecture.ThumbnailUrl);
        }

        if (lectures.Count > 0)
        {
            _unitOfWork.Lectures.RemoveRange(lectures);
        }
    }

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
