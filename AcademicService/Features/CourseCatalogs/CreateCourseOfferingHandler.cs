using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public class CreateCourseOfferingHandler : IRequestHandler<CreateCourseOfferingCommand, CourseResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public CreateCourseOfferingHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<CourseResponse> Handle(
        CreateCourseOfferingCommand request,
        CancellationToken cancellationToken)
    {
        var catalog = await _unitOfWork.CourseCatalogs.GetByIdAsync(request.CourseCatalogId);
        if (catalog is null)
            throw new KeyNotFoundException($"Course catalog {request.CourseCatalogId} not found.");

        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department is null)
            throw new KeyNotFoundException($"Department {request.DepartmentId} not found.");

        if (await _unitOfWork.Courses.AnyAsync(course =>
                course.CourseCatalogId == request.CourseCatalogId &&
                course.DepartmentId == request.DepartmentId &&
                course.IsActive))
        {
            throw new InvalidOperationException(
                $"An active offering already exists for catalog {request.CourseCatalogId} in department {request.DepartmentId}.");
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = catalog.Name,
            Description = catalog.Description,
            CoverImageUrl = catalog.CoverImageUrl,
            DoctorId = request.DoctorId,
            DepartmentId = request.DepartmentId,
            CourseCatalogId = request.CourseCatalogId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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
            course.CourseCatalogId);
    }
}
