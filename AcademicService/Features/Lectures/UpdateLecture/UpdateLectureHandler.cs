using AcademicService.Contracts;
using AcademicService.Features.Lectures.GetLecturesByCourse;
using MediatR;

namespace AcademicService.Features.Lectures.UpdateLecture;

public class UpdateLectureHandler : IRequestHandler<UpdateLectureCommand, LectureResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public UpdateLectureHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<LectureResponse> Handle(UpdateLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        if (request.Thumbnail is not null)
        {
            // Delete old thumbnail first
            if (!string.IsNullOrWhiteSpace(lecture.ThumbnailUrl))
                _imageHelper.DeleteImage(lecture.ThumbnailUrl);

            lecture.ThumbnailUrl = await _imageHelper.SaveImageAsync(request.Thumbnail, "Lectures");
        }

        lecture.Title = request.Title;
        lecture.OrderIndex = request.OrderIndex;
        lecture.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Lectures.Update(lecture);
        await _unitOfWork.SaveChangesAsync();

        return new LectureResponse(
            lecture.Id,
            lecture.Title,
            lecture.OrderIndex,
            _imageHelper.GetImageUrl(lecture.ThumbnailUrl ?? string.Empty) ?? string.Empty,
            lecture.CourseId
        );
    }
}
