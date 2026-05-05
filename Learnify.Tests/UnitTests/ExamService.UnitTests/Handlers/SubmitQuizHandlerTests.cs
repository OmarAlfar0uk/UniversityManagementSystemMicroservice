using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Data.Enums;
using ExamService.Features.Quiz.SubmitQuiz;
using Moq;
using FluentAssertions;
using MassTransit;
using Shered.Events;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace ExamService.UnitTests.Handlers;

public class SubmitQuizHandlerTests
{
    private readonly Mock<IUnitOfWork>               _uow             = new();
    private readonly Mock<IGenericRepository<Quiz>>        _quizRepo        = new();
    private readonly Mock<IGenericRepository<QuizAttempt>> _attemptRepo = new();
    private readonly Mock<IGenericRepository<QuizAnswer>>   _answerRepo   = new();
    private readonly Mock<IPublishEndpoint>          _publishEndpoint = new();
    private readonly SubmitQuizHandler               _handler;

    public SubmitQuizHandlerTests()
    {
        _uow.Setup(x => x.Quizzes).Returns(_quizRepo.Object);
        _uow.Setup(x => x.QuizAttempts).Returns(_attemptRepo.Object);
        _uow.Setup(x => x.QuizAnswers).Returns(_answerRepo.Object);
        _handler = new SubmitQuizHandler(_uow.Object, _publishEndpoint.Object, Mock.Of<ILogger<SubmitQuizHandler>>());
    }

    [Fact]
    public async Task Handle_QuizNotFound_ThrowsKeyNotFoundException()
    {
        _quizRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Quiz, bool>>>())).ReturnsAsync((Quiz?)null);
        var command = new SubmitQuizCommand(Guid.NewGuid(), Guid.NewGuid(), []);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_MaxAttemptsReached_ThrowsInvalidOperationException()
    {
        var quizId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var lectureId = Guid.NewGuid();
        var quiz = new Quiz { Id = quizId, LectureId = lectureId, MaxAttempts = 1 };
        
        _quizRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Quiz, bool>>>())).ReturnsAsync(quiz);
        _attemptRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<QuizAttempt, bool>>>())).ReturnsAsync(true);
        
        var command = new SubmitQuizCommand(lectureId, studentId, []);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<InvalidOperationException>();
    }
}
