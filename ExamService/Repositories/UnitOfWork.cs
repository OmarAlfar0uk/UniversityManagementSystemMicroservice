using ExamService.Contracts;
using ExamService.Data;
using ExamService.Data.Models;

namespace ExamService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ExamDbContext _context;

    private IGenericRepository<Quiz>? _quizzes;
    private IGenericRepository<QuizQuestion>? _quizQuestions;
    private IGenericRepository<QuizQuestionOption>? _quizQuestionOptions;
    private IGenericRepository<QuizAttempt>? _quizAttempts;
    private IGenericRepository<QuizAnswer>? _quizAnswers;

    public UnitOfWork(ExamDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Quiz> Quizzes =>
        _quizzes ??= new GenericRepository<Quiz>(_context);

    public IGenericRepository<QuizQuestion> QuizQuestions =>
        _quizQuestions ??= new GenericRepository<QuizQuestion>(_context);

    public IGenericRepository<QuizQuestionOption> QuizQuestionOptions =>
        _quizQuestionOptions ??= new GenericRepository<QuizQuestionOption>(_context);

    public IGenericRepository<QuizAttempt> QuizAttempts =>
        _quizAttempts ??= new GenericRepository<QuizAttempt>(_context);

    public IGenericRepository<QuizAnswer> QuizAnswers =>
        _quizAnswers ??= new GenericRepository<QuizAnswer>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
