namespace GradeService.Features.Grades.GetGPA;

public record GPAResponse(Guid StudentId, decimal GPA, int CoursesGraded);
