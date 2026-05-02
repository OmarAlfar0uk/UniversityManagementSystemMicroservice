using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseById;

public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetCourseByIdHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<CourseDetailsResponse> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        var lectures = await _unitOfWork.Lectures
            .GetAllAsync(l => l.CourseId == request.CourseId);

        return new CourseDetailsResponse(
            course.Id,
            course.Name,
            course.Description ?? string.Empty,
            _imageHelper.GetImageUrl(course.CoverImageUrl ?? string.Empty) ?? string.Empty,
            course.DoctorId,
            lectures.Count(),
            0m,
            course.DepartmentId,
            course.CourseCatalogId
        );
    }
}
