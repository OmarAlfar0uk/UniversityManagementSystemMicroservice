using AcademicService.Contracts;
using AcademicService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public class GetCourseStudentsDetailsHandler : IRequestHandler<GetCourseStudentsDetailsQuery, List<StudentDetailItem>>
{
    private readonly AcademicDbContext _dbContext;
    private readonly IAuthServiceClient _authServiceClient;

    public GetCourseStudentsDetailsHandler(
        AcademicDbContext dbContext,
        IAuthServiceClient authServiceClient)
    {
        _dbContext = dbContext;
        _authServiceClient = authServiceClient;
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

        var students = new List<StudentDetailItem>(studentIds.Count);
        foreach (var studentId in studentIds)
        {
            var userInfo = await _authServiceClient.GetUserInfoAsync(studentId);
            var fullName = userInfo?.FullName ?? string.Empty;
            var firstName = userInfo?.FirstName ?? GetFirstName(fullName);

            students.Add(new StudentDetailItem(
                StudentId: studentId,
                FirstName: firstName,
                FullName: fullName,
                MidtermGrade: null,
                FinalGrade: null,
                AttendedLecturesCount: 0
            ));
        }

        return students;
    }

    private static string GetFirstName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        return fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
    }
}
