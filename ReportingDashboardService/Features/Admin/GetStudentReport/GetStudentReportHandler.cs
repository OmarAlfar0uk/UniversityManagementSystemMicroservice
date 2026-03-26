using MediatR;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Admin.GetStudentReport
{
    public class GetStudentReportHandler : IRequestHandler<GetStudentReportQuery, StudentSummaryResponse>
    {
        private readonly IAcademicServiceClient _academic;
        private readonly IAttendanceServiceClient _attendance;
        private readonly IGradeServiceClient _grade;
        private readonly IExamServiceClient _exam;

        public GetStudentReportHandler(
            IAcademicServiceClient academic,
            IAttendanceServiceClient attendance,
            IGradeServiceClient grade,
            IExamServiceClient exam)
        {
            _academic   = academic;
            _attendance = attendance;
            _grade      = grade;
            _exam       = exam;
        }

        public async Task<StudentSummaryResponse> Handle(
            GetStudentReportQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = request.StudentId;

            var coursesTask     = _academic.GetEnrolledCoursesAsync(studentId);
            var attendanceTask  = _attendance.GetStudentAttendanceAsync(studentId);
            var gradesTask      = _grade.GetStudentGradesAsync(studentId);
            var gpaTask         = _grade.GetStudentGpaAsync(studentId);
            var quizResultsTask = _exam.GetStudentQuizResultsAsync(studentId);

            await Task.WhenAll(coursesTask, attendanceTask, gradesTask, gpaTask, quizResultsTask);

            var courses     = coursesTask.Result;
            var attendance  = attendanceTask.Result;
            var grades      = gradesTask.Result;
            var gpa         = gpaTask.Result;
            var quizResults = quizResultsTask.Result;

            var attendanceLookup = attendance.ToDictionary(a => a.CourseId);
            var gradesLookup     = grades.ToDictionary(g => g.CourseId);

            double overallAttendance = attendance.Any()
                ? attendance.Average(a => a.Percentage) : 0.0;

            var courseItems = courses.Select(c =>
            {
                attendanceLookup.TryGetValue(c.Id, out var att);
                gradesLookup.TryGetValue(c.Id, out var grade);

                var totalScore = grade?.TotalScore;
                var label = totalScore switch
                {
                    >= 85 => "Excellent",
                    >= 70 => "Good",
                    >= 50 => "At Risk",
                    _     => "No Data"
                };

                return new CourseSummaryItem(
                    c.Id, c.Name, c.CoverImageUrl,
                    att?.AttendedCount ?? 0, att?.TotalLectures ?? 0, att?.Percentage ?? 0.0,
                    grade?.MidtermScore, grade?.FinalScore, totalScore, label);
            }).ToList();

            return new StudentSummaryResponse(
                courses.Count, overallAttendance, gpa,
                quizResults.Count, quizResults.Count(q => q.IsPassed), courseItems);
        }
    }
}
