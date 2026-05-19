using AcademicService.Contracts;
using AcademicService.Features.Courses;
using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.Courses.UpdateCourse;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, CourseResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;
    private readonly IAuthServiceClient _authServiceClient;

    public UpdateCourseHandler(
        IUnitOfWork unitOfWork,
        IImageHelper imageHelper,
        IAuthServiceClient authServiceClient)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
        _authServiceClient = authServiceClient;
    }

    public async Task<CourseResponse> Handle(
        UpdateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        if (request.CoverImage is not null)
        {
            // Delete old cover image first
            if (!string.IsNullOrWhiteSpace(course.CoverImageUrl))
                _imageHelper.DeleteImage(course.CoverImageUrl);

            course.CoverImageUrl = await _imageHelper.SaveImageAsync(request.CoverImage, "Courses");
        }

        course.Name = request.Name;
        course.Description = request.Description;
        course.DoctorId = request.DoctorId;
        course.UpdatedAt = DateTime.UtcNow;

        await CourseDoctorInfoMapper.EnrichAsync(course, _authServiceClient);

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return new CourseResponse(
            course.Id,
            course.Name,
            course.Description ?? string.Empty,
            _imageHelper.GetImageUrl(course.CoverImageUrl ?? string.Empty) ?? string.Empty,
            course.DoctorId,
            course.DoctorFirstName,
            course.DoctorFullName,
            0m,
            course.DepartmentId,
            course.CourseCatalogId
        );
    }
}
