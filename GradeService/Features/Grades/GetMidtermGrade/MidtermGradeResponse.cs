namespace GradeService.Features.Grades.GetMidtermGrade;

public record MidtermGradeResponse(Guid CourseId, Guid StudentId, decimal? Score, string? LetterGrade);
