using MediatR;
using ReportingDashboardService.Contracts;

namespace ReportingDashboardService.Features.Doctor.GetDoctorDashboard
{
    public class GetDoctorDashboardHandler
        : IRequestHandler<GetDoctorDashboardQuery, DoctorDashboardResponse>
    {
        private readonly IAcademicServiceClient _academic;
        private readonly IAttendanceServiceClient _attendance;
        private readonly IExamServiceClient _exam;

        public GetDoctorDashboardHandler(
            IAcademicServiceClient academic,
            IAttendanceServiceClient attendance,
            IExamServiceClient exam)
        {
            _academic   = academic;
            _attendance = attendance;
            _exam       = exam;
        }

        public async Task<DoctorDashboardResponse> Handle(
            GetDoctorDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var doctorId = request.DoctorId;

            // Get doctor's courses and pending essays in parallel
            var coursesTask       = _academic.GetDoctorCoursesAsync(doctorId);
            var pendingEssaysTask = _exam.GetPendingEssaysCountAsync(doctorId);

            await Task.WhenAll(coursesTask, pendingEssaysTask);

            var courses       = coursesTask.Result;
            var pendingEssays = pendingEssaysTask.Result;

            // Parallel per-course calls
            var attendanceTasks = courses.Select(c => _attendance.GetCourseAttendanceAverageAsync(c.Id)).ToList();
            var enrollmentTasks = courses.Select(c => _academic.GetCourseEnrollmentCountAsync(c.Id)).ToList();

            await Task.WhenAll(attendanceTasks);
            await Task.WhenAll(enrollmentTasks);

            var attendanceAverages = attendanceTasks.Select(t => t.Result).ToList();
            var enrollmentCounts   = enrollmentTasks.Select(t => t.Result).ToList();

            double overallAttendanceAverage = attendanceAverages.Any()
                ? attendanceAverages.Average() : 0.0;

            int totalStudents = enrollmentCounts.Sum();

            var courseStats = courses.Select((c, i) => new DoctorCourseStats(
                CourseId:         c.Id,
                CourseName:       c.Name,
                CoverImageUrl:    c.CoverImageUrl,
                EnrolledStudents: enrollmentCounts[i],
                AttendanceAverage:attendanceAverages[i]
            )).ToList();

            return new DoctorDashboardResponse(
                TotalCourses:            courses.Count,
                TotalStudents:           totalStudents,
                OverallAttendanceAverage:overallAttendanceAverage,
                PendingEssaysToGrade:    pendingEssays,
                Courses:                 courseStats
            );
        }
    }
}
