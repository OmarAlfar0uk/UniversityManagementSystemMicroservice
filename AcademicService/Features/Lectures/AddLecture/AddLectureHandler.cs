using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Lectures.GetLecturesByCourse;
using MassTransit;
using MediatR;
using Shered.Events;

namespace AcademicService.Features.Lectures.AddLecture;

public class AddLectureHandler : IRequestHandler<AddLectureCommand, LectureResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddLectureHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
        _publishEndpoint = publishEndpoint;
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

        // ✅ Load enrolled students and publish event
        var enrollments = await _unitOfWork.CourseEnrollments.GetAllAsync(
            e => e.CourseId == request.CourseId);
        var studentIds = enrollments.Select(e => e.StudentId).ToList();

        await _publishEndpoint.Publish<ILectureAdded>(new
        {
            LectureId    = lecture.Id,
            CourseId     = course.Id,
            CourseName   = course.Name,
            LectureTitle = lecture.Title,
            StudentIds   = studentIds
        }, cancellationToken);

        return new LectureResponse(
            lecture.Id,
            lecture.Title,
            lecture.OrderIndex,
            _imageHelper.GetImageUrl(lecture.ThumbnailUrl ?? string.Empty) ?? string.Empty,
            lecture.CourseId
        );
    }
}

