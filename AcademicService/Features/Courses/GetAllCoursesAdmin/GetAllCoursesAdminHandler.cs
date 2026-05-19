using AcademicService.Contracts;
using AcademicService.Features.Courses;
using MediatR;

namespace AcademicService.Features.Courses.GetAllCoursesAdmin;

public class GetAllCoursesAdminHandler
    : IRequestHandler<GetAllCoursesAdminQuery, GetAllCoursesAdminResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;
    private readonly IAuthServiceClient _authServiceClient;

    public GetAllCoursesAdminHandler(
        IUnitOfWork unitOfWork,
        IImageHelper imageHelper,
        IAuthServiceClient authServiceClient)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
        _authServiceClient = authServiceClient;
    }

    public async Task<GetAllCoursesAdminResponse> Handle(
        GetAllCoursesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var courses = (await _unitOfWork.Courses.GetAllAsync()).ToList();
        await CourseDoctorInfoMapper.EnrichAsync(courses, _authServiceClient);

        var items = new List<AdminCourseItem>();

        foreach (var course in courses)
        {
            var enrolledCount = await _unitOfWork.CourseEnrollments
                .CountAsync(e => e.CourseId == course.Id);

            items.Add(new AdminCourseItem(
                Id: course.Id,
                Name: course.Name,
                DoctorId: course.DoctorId,
                DoctorFirstName: course.DoctorFirstName ?? string.Empty,
                DoctorFullName: course.DoctorFullName ?? string.Empty,
                CoverImageUrl: _imageHelper.GetImageUrl(course.CoverImageUrl ?? string.Empty),
                EnrolledCount: enrolledCount,
                DepartmentId: course.DepartmentId
            ));
        }

        return new GetAllCoursesAdminResponse(items);
    }
}
