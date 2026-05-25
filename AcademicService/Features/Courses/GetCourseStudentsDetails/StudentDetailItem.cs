namespace AcademicService.Features.Courses.GetCourseStudentsDetails;

public record StudentDetailItem(
    Guid StudentId,
    string? FirstName,
    string? FullName,
    double? MidtermGrade,
    double? FinalGrade,
    int AttendedLecturesCount
);
