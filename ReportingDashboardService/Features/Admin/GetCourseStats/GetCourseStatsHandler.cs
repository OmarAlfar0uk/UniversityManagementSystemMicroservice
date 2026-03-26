using MediatR;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Responses;

namespace ReportingDashboardService.Features.Admin.GetCourseStats
{
    public class GetCourseStatsHandler : IRequestHandler<GetCourseStatsQuery, CourseStatsResponse>
    {
        private readonly IAcademicServiceClient _academic;
        private readonly IAttendanceServiceClient _attendance;
        private readonly IGradeServiceClient _grade;
        private readonly IExamServiceClient _exam;

        public GetCourseStatsHandler(
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

        public async Task<CourseStatsResponse> Handle(
            GetCourseStatsQuery request,
            CancellationToken cancellationToken)
        {
            var courseId = request.CourseId;

            // Parallel calls
            var lecturesTask        = _academic.GetCourseLecturesAsync(courseId);
            var enrollmentTask      = _academic.GetCourseEnrollmentCountAsync(courseId);
            var attendanceTask      = _attendance.GetCourseAttendanceAsync(courseId);
            var gradesTask          = _grade.GetCourseGradesAsync(courseId);
            var avgGradeTask        = _grade.GetCourseAverageGradeAsync(courseId);
            var quizStatsTask       = _exam.GetCourseQuizStatsAsync(courseId);

            await Task.WhenAll(
                lecturesTask, enrollmentTask, attendanceTask,
                gradesTask, avgGradeTask, quizStatsTask);

            var lectures    = lecturesTask.Result;
            var enrollment  = enrollmentTask.Result;
            var attendance  = attendanceTask.Result;
            var grades      = gradesTask.Result;
            var avgGrade    = avgGradeTask.Result;
            var quizStats   = quizStatsTask.Result;

            // Derive course name from first attendance entry if possible
            // (No separate course endpoint — use a placeholder)
            const string courseName = "Course";

            double attendanceAverage = attendance.Any()
                ? attendance.Average(a => a.Percentage) : 0.0;

            int passedStudents = grades.Count(g => g.MidtermScore + g.FinalScore >= 50);
            int failedStudents = grades.Count - passedStudents;

            var quizStatItems = quizStats.Select(q => new QuizStatItem(
                QuizId:       q.QuizId,
                AverageScore: q.AverageScore,
                TotalAttempts:q.TotalAttempts,
                PassedCount:  q.PassedCount,
                PassRate:     q.TotalAttempts > 0
                    ? Math.Round((double)q.PassedCount / q.TotalAttempts * 100, 2)
                    : 0.0
            )).ToList();

            return new CourseStatsResponse(
                CourseId:         courseId,
                CourseName:       courseName,
                TotalLectures:    lectures.Count,
                EnrolledStudents: enrollment,
                AttendanceAverage:attendanceAverage,
                GradeAverage:     avgGrade,
                PassedStudents:   passedStudents,
                FailedStudents:   failedStudents,
                QuizStats:        quizStatItems
            );
        }
    }
}
