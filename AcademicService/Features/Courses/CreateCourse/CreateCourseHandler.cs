using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Courses;
using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.Courses.CreateCourse;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, CourseResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;
    private readonly IAuthServiceClient _authServiceClient;

    public CreateCourseHandler(
        IUnitOfWork unitOfWork,
        IImageHelper imageHelper,
        IAuthServiceClient authServiceClient)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
        _authServiceClient = authServiceClient;
    }

    public async Task<CourseResponse> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken)
    {
        string? coverImageUrl = null;
        if (request.CoverImage is not null)
            coverImageUrl = await _imageHelper.SaveImageAsync(request.CoverImage, "Courses");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CoverImageUrl = coverImageUrl,
            DoctorId = request.DoctorId,
            DepartmentId = request.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await CourseDoctorInfoMapper.EnrichAsync(course, _authServiceClient);

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return new CourseResponse(
            course.Id,
            course.Name,
            course.Description ?? string.Empty,
            _imageHelper.GetImageUrl(course.CoverImageUrl ?? string.Empty) ?? string.Empty,
            course.DoctorId,
            course.DoctorFirstName ?? string.Empty,
            course.DoctorFullName ?? string.Empty,
            0m,
            course.DepartmentId,
            course.CourseCatalogId
        );
    }
}
