using AcademicService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public class GetCourseStudentsDetailsHandler : IRequestHandler<GetCourseStudentsDetailsQuery, List<StudentDetailItem>>
{
    private readonly AcademicDbContext _dbContext;

    public GetCourseStudentsDetailsHandler(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<StudentDetailItem>> Handle(
        GetCourseStudentsDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var studentIds = await _dbContext.CourseEnrollments
            .AsNoTracking()
            .Where(e => e.CourseId == request.CourseId)
            .Select(e => e.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return studentIds
            .Select(studentId => new StudentDetailItem(
                StudentId: studentId,
                FirstName: null,
                FullName: null,
                MidtermGrade: null,
                FinalGrade: null,
                AttendedLecturesCount: 0
            ))
            .ToList();
    }
}
