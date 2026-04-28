using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetAllCoursesAdmin;

public class GetAllCoursesAdminHandler
    : IRequestHandler<GetAllCoursesAdminQuery, GetAllCoursesAdminResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetAllCoursesAdminHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<GetAllCoursesAdminResponse> Handle(
        GetAllCoursesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetAllAsync();
        var items = new List<AdminCourseItem>();

        foreach (var course in courses)
        {
            var enrolledCount = await _unitOfWork.CourseEnrollments
                .CountAsync(e => e.CourseId == course.Id);

            items.Add(new AdminCourseItem(
                Id: course.Id,
                Name: course.Name,
                DoctorId: course.DoctorId,
                CoverImageUrl: _imageHelper.GetImageUrl(course.CoverImageUrl ?? string.Empty),
                EnrolledCount: enrolledCount,
                DepartmentId: course.DepartmentId
            ));
        }

        return new GetAllCoursesAdminResponse(items);
    }
}
