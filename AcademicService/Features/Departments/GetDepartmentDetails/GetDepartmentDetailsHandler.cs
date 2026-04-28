using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Departments.GetDepartmentDetails;

public class GetDepartmentDetailsHandler
    : IRequestHandler<GetDepartmentDetailsQuery, DepartmentDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDepartmentDetailsHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<DepartmentDetailsResponse> Handle(
        GetDepartmentDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments
            .GetByIdAsync(request.DepartmentId)
            ?? throw new KeyNotFoundException($"Department {request.DepartmentId} not found.");

        var courses = await _unitOfWork.Courses
            .GetAllAsync(c => c.DepartmentId == request.DepartmentId);

        var courseItems = new List<DepartmentCourseItem>();
        var allStudentIds = new HashSet<Guid>();

        foreach (var course in courses)
        {
            var enrollments = await _unitOfWork.CourseEnrollments
                .GetAllAsync(e => e.CourseId == course.Id);

            foreach (var e in enrollments)
                allStudentIds.Add(e.StudentId);

            courseItems.Add(new DepartmentCourseItem(
                Id: course.Id,
                Name: course.Name,
                EnrolledStudentsCount: enrollments.Count()
            ));
        }

        return new DepartmentDetailsResponse(
            Id: department.Id,
            Name: department.Name,
            Code: department.Code,
            StudentsCount: allStudentIds.Count,
            Courses: courseItems
        );
    }
}
