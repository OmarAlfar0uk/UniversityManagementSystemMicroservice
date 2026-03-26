using MediatR;
using ReportingDashboardService.Contracts;

namespace ReportingDashboardService.Features.Parent.GetChildAttendanceReport
{
    public class GetChildAttendanceReportHandler
        : IRequestHandler<GetChildAttendanceReportQuery, ChildAttendanceReportResponse>
    {
        private readonly IAttendanceServiceClient _attendance;
        private readonly IAcademicServiceClient _academic;

        public GetChildAttendanceReportHandler(
            IAttendanceServiceClient attendance,
            IAcademicServiceClient academic)
        {
            _attendance = attendance;
            _academic   = academic;
        }

        public async Task<ChildAttendanceReportResponse> Handle(
            GetChildAttendanceReportQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = request.StudentId;

            var attendanceTask = _attendance.GetStudentAttendanceAsync(studentId);
            var coursesTask    = _academic.GetEnrolledCoursesAsync(studentId);

            await Task.WhenAll(attendanceTask, coursesTask);

            var attendance = attendanceTask.Result;
            var courses    = coursesTask.Result;

            var coursesLookup = courses.ToDictionary(c => c.Id);

            double overall = attendance.Any() ? attendance.Average(a => a.Percentage) : 0.0;

            var courseReports = attendance.Select(a =>
            {
                coursesLookup.TryGetValue(a.CourseId, out var course);
                var statusLabel = a.Percentage switch
                {
                    >= 75 => "Good",
                    >= 50 => "Warning",
                    _     => "Critical"
                };

                return new CourseAttendanceReport(
                    CourseId:        a.CourseId,
                    CourseName:      course?.Name ?? "Unknown",
                    TotalLectures:   a.TotalLectures,
                    AttendedLectures:a.AttendedCount,
                    Percentage:      a.Percentage,
                    StatusLabel:     statusLabel
                );
            }).ToList();

            return new ChildAttendanceReportResponse(studentId, overall, courseReports);
        }
    }
}
