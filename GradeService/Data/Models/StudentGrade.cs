namespace GradeService.Data.Models;

public class StudentGrade
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public decimal AttendanceScore { get; set; }
    public decimal AssignmentScore { get; set; }
    public decimal QuizScore { get; set; }
    public decimal MidtermScore { get; set; }
    public decimal FinalScore { get; set; }
    public decimal TotalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
