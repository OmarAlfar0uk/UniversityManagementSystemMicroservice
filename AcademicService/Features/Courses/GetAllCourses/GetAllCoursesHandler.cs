using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetAllCourses;

public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, PagedResponse<CourseResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCoursesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<CourseResponse>> Handle(
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        // Fetch enrollments for this student
        var enrollments = await _unitOfWork.CourseEnrollments
            .GetAllAsync(e => e.StudentId == request.StudentId);

        var courseIds = enrollments.Select(e => e.CourseId).ToList();

        // Fetch all enrolled courses
        var allCourses = await _unitOfWork.Courses
            .GetAllAsync(c => courseIds.Contains(c.Id) && c.IsActive);

        var totalCount = allCourses.Count();

        var paged = allCourses
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseResponse(
                c.Id,
                c.Name,
                c.Description ?? string.Empty,
                c.CoverImageUrl ?? string.Empty,
                c.DoctorId,
                0m,
                c.DepartmentId,
                c.CourseCatalogId
            ));

        return new PagedResponse<CourseResponse>(
            paged,
            request.PageNumber,
            request.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}
