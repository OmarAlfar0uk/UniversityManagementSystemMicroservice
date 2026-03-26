using MediatR;
using ReportingDashboardService.Contracts;

namespace ReportingDashboardService.Features.Admin.GetAdminDashboard
{
    public class GetAdminDashboardHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardResponse>
    {
        private readonly IAuthServiceClient _auth;
        private readonly IAcademicServiceClient _academic;
        private readonly IAttendanceServiceClient _attendance;

        public GetAdminDashboardHandler(
            IAuthServiceClient auth,
            IAcademicServiceClient academic,
            IAttendanceServiceClient attendance)
        {
            _auth       = auth;
            _academic   = academic;
            _attendance = attendance;
        }

        public async Task<AdminDashboardResponse> Handle(
            GetAdminDashboardQuery request,
            CancellationToken cancellationToken)
        {
            // Parallel first-level calls
            var studentsCountTask = _auth.GetTotalUsersCountAsync("Student");
            var doctorsCountTask  = _auth.GetTotalUsersCountAsync("Doctor");
            var coursesCountTask  = _academic.GetTotalCoursesCountAsync();
            var allCoursesTask    = _academic.GetAllCoursesAsync();

            await Task.WhenAll(studentsCountTask, doctorsCountTask, coursesCountTask, allCoursesTask);

            var totalStudents = studentsCountTask.Result;
            var totalDoctors  = doctorsCountTask.Result;
            var totalCourses  = coursesCountTask.Result;
            var allCourses    = allCoursesTask.Result;

            // Parallel attendance average calls for each course
            var attendanceTasks = allCourses
                .Select(c => _attendance.GetCourseAttendanceAverageAsync(c.Id))
                .ToList();

            var attendanceAverages = await Task.WhenAll(attendanceTasks);

            // Build course attendance pairs
            var courseAttendancePairs = allCourses
                .Zip(attendanceAverages, (course, avg) => (course, avg))
                .ToList();

            double systemAttendanceAverage = attendanceAverages.Any()
                ? attendanceAverages.Average()
                : 0.0;

            var topByEnrollment = allCourses
                .OrderByDescending(c => c.EnrolledCount)
                .Take(5)
                .Select(c => new CourseEnrollmentStat(c.Id, c.Name, c.EnrolledCount))
                .ToList();

            var lowestAttendance = courseAttendancePairs
                .OrderBy(p => p.avg)
                .Take(5)
                .Select(p => new CourseAttendanceStat(p.course.Id, p.course.Name, p.avg))
                .ToList();

            return new AdminDashboardResponse(
                totalStudents,
                totalDoctors,
                totalCourses,
                systemAttendanceAverage,
                topByEnrollment,
                lowestAttendance
            );
        }
    }
}
