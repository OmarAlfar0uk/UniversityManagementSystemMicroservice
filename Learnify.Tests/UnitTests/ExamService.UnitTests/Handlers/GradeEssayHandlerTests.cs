using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Features.Quiz.GradeEssay;
using Moq;
using FluentAssertions;
using MassTransit;
using Shered.Events;

namespace ExamService.UnitTests.Handlers;

public class GradeEssayHandlerTests
{
    private readonly Mock<IUnitOfWork>               _uow             = new();
    private readonly Mock<IGenericRepository<QuizAttempt>> _repo      = new();
    private readonly Mock<IGenericRepository<Quiz>>  _quizRepo      = new();
    private readonly Mock<IPublishEndpoint>          _publishEndpoint = new();
    private readonly GradeEssayHandler               _handler;

    public GradeEssayHandlerTests()
    {
        _uow.Setup(x => x.QuizAttempts).Returns(_repo.Object);
        _uow.Setup(x => x.Quizzes).Returns(_quizRepo.Object);
        _handler = new GradeEssayHandler(_uow.Object, _publishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_AttemptNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((QuizAttempt?)null);
        var command = new GradeEssayCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5m);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesScoreAndPublishesEvent()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var attempt = new QuizAttempt
        {
            Id = attemptId,
            StudentId = studentId,
            Score = 10m,
            Quiz = new Quiz { Id = Guid.NewGuid() },
            Answers = new List<QuizAnswer>
            {
                new QuizAnswer { QuizQuestionId = questionId, EarnedPoints = 0m, IsCorrect = false }
            }
        };

        _repo.Setup(x => x.GetByIdAsync(attemptId)).ReturnsAsync(attempt);
        _quizRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(attempt.Quiz);
        _repo.Setup(x => x.Update(It.IsAny<QuizAttempt>()));
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish<IQuizCompleted>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new GradeEssayCommand(attempt.Quiz.Id, attemptId, questionId, 5m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        attempt.Score.Should().Be(15m); // 10 original + 5 new
        attempt.Answers.First().EarnedPoints.Should().Be(5m);
        attempt.Answers.First().IsCorrect.Should().BeTrue();
        
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
        _publishEndpoint.Verify(x => x.Publish<IQuizCompleted>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
