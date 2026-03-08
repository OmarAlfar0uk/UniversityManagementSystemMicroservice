using ExamService.Data.Models;

namespace ExamService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Quiz> Quizzes { get; }
    IGenericRepository<QuizQuestion> QuizQuestions { get; }
    IGenericRepository<QuizQuestionOption> QuizQuestionOptions { get; }
    IGenericRepository<QuizAttempt> QuizAttempts { get; }
    IGenericRepository<QuizAnswer> QuizAnswers { get; }
    Task<int> SaveChangesAsync();
}
