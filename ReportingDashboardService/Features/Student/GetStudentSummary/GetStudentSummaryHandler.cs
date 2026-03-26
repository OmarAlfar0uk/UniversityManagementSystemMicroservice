using MediatR;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Student.GetStudentSummary
{
    public class GetStudentSummaryHandler : IRequestHandler<GetStudentSummaryQuery, StudentSummaryResponse>
    {
        private readonly IAcademicServiceClient _academic;
        private readonly IAttendanceServiceClient _attendance;
        private readonly IGradeServiceClient _grade;
        private readonly IExamServiceClient _exam;

        public GetStudentSummaryHandler(
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
            GetStudentSummaryQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = request.StudentId;

            // Parallel calls to downstream services
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

            // Build attendance lookup
            var attendanceLookup = attendance.ToDictionary(a => a.CourseId);

            // Build grades lookup
            var gradesLookup = grades.ToDictionary(g => g.CourseId);

            // Overall attendance
            double overallAttendance = attendance.Any()
                ? attendance.Average(a => a.Percentage)
                : 0.0;

            // Course summary items
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
                    CourseId:            c.Id,
                    CourseName:          c.Name,
                    CoverImageUrl:       c.CoverImageUrl,
                    AttendedLectures:    att?.AttendedCount    ?? 0,
                    TotalLectures:       att?.TotalLectures    ?? 0,
                    AttendancePercentage:att?.Percentage       ?? 0.0,
                    MidtermScore:        grade?.MidtermScore,
                    FinalScore:          grade?.FinalScore,
                    TotalScore:          totalScore,
                    PerformanceLabel:    label
                );
            }).ToList();

            return new StudentSummaryResponse(
                TotalEnrolledCourses:       courses.Count,
                OverallAttendancePercentage: overallAttendance,
                GPA:                        gpa,
                TotalQuizzesTaken:          quizResults.Count,
                QuizzesPassed:              quizResults.Count(q => q.IsPassed),
                Courses:                    courseItems
            );
        }
    }
}
