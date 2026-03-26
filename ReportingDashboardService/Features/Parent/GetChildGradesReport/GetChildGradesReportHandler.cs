using MediatR;
using ReportingDashboardService.Contracts;

namespace ReportingDashboardService.Features.Parent.GetChildGradesReport
{
    public class GetChildGradesReportHandler
        : IRequestHandler<GetChildGradesReportQuery, ChildGradesReportResponse>
    {
        private readonly IGradeServiceClient _grade;
        private readonly IAcademicServiceClient _academic;

        public GetChildGradesReportHandler(
            IGradeServiceClient grade,
            IAcademicServiceClient academic)
        {
            _grade    = grade;
            _academic = academic;
        }

        public async Task<ChildGradesReportResponse> Handle(
            GetChildGradesReportQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = request.StudentId;

            var gradesTask  = _grade.GetStudentGradesAsync(studentId);
            var gpaTask     = _grade.GetStudentGpaAsync(studentId);
            var coursesTask = _academic.GetEnrolledCoursesAsync(studentId);

            await Task.WhenAll(gradesTask, gpaTask, coursesTask);

            var grades  = gradesTask.Result;
            var gpa     = gpaTask.Result;
            var courses = coursesTask.Result;

            var coursesLookup = courses.ToDictionary(c => c.Id);

            var gpaLabel = gpa switch
            {
                >= 3.7 => "Excellent",
                >= 3.0 => "Very Good",
                >= 2.0 => "Good",
                >= 1.0 => "Pass",
                _ => "Fail"
            };

            var courseGradeReports = grades.Select(g =>
            {
                coursesLookup.TryGetValue(g.CourseId, out var course);

                var gradeLetter = g.TotalScore switch
                {
                    >= 90 => "A",
                    >= 80 => "B",
                    >= 70 => "C",
                    >= 60 => "D",
                    not null => "F",
                    null => "N/A"
                };

                return new CourseGradeReport(
                    CourseId:     g.CourseId,
                    CourseName:   course?.Name ?? "Unknown",
                    MidtermScore: g.MidtermScore,
                    FinalScore:   g.FinalScore,
                    TotalScore:   g.TotalScore,
                    Grade:        gradeLetter
                );
            }).ToList();

            return new ChildGradesReportResponse(studentId, gpa, gpaLabel, courseGradeReports);
        }
    }
}
