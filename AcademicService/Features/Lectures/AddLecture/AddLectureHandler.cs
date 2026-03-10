using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Lectures.GetLecturesByCourse;
using MediatR;

namespace AcademicService.Features.Lectures.AddLecture;

public class AddLectureHandler : IRequestHandler<AddLectureCommand, LectureResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public AddLectureHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<LectureResponse> Handle(AddLectureCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        string? thumbnailUrl = null;
        if (request.Thumbnail is not null)
            thumbnailUrl = await _imageHelper.SaveImageAsync(request.Thumbnail, "Lectures");

        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            Title = request.Title,
            OrderIndex = request.OrderIndex,
            ThumbnailUrl = thumbnailUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Lectures.AddAsync(lecture);
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
