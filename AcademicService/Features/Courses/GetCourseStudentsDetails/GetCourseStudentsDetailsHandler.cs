using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public class GetCourseStudentsDetailsHandler : IRequestHandler<GetCourseStudentsDetailsQuery, List<StudentDetailItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseStudentsDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StudentDetailItem>> Handle(
        GetCourseStudentsDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var enrollments = await _unitOfWork.CourseEnrollments
            .GetAllAsync(e => e.CourseId == request.CourseId);

        return enrollments.Select(e => new StudentDetailItem(
            StudentId: e.StudentId,
            FullName: null,       // cross-service — Frontend resolves
            MidtermGrade: null,   // cross-service
            FinalGrade: null,     // cross-service
            AttendedLecturesCount: 0  // cross-service
        )).ToList();
    }
}
