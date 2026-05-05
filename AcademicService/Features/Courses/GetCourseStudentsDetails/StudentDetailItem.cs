namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public record StudentDetailItem(
    Guid StudentId,
    string? FullName,
    double? MidtermGrade,
    double? FinalGrade,
    int AttendedLecturesCount
);
