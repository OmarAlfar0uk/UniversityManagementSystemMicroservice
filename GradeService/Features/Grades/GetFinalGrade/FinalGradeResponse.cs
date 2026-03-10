namespace GradeService.Features.Grades.GetFinalGrade;

public record FinalGradeResponse(Guid CourseId, Guid StudentId, decimal? Score, string? LetterGrade, decimal? GradePoints);
