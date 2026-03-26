using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Data.Enums;
using ExamService.Features.Quiz.SubmitQuiz;
using Moq;
using FluentAssertions;
using MassTransit;
using Shered.Events;

namespace ExamService.UnitTests.Handlers;

public class SubmitQuizHandlerTests
{
    private readonly Mock<IUnitOfWork>               _uow             = new();
    private readonly Mock<IGenericRepository<Quiz>>  _quizRepo        = new();
    private readonly Mock<IGenericRepository<QuizAttempt>> _attemptRepo = new();
    private readonly Mock<IGenericRepository<QuizAnswer>> _answerRepo = new();
    private readonly Mock<IPublishEndpoint>          _publishEndpoint = new();
    private readonly SubmitQuizHandler               _handler;

    public SubmitQuizHandlerTests()
    {
        _uow.Setup(x => x.Quizzes).Returns(_quizRepo.Object);
        _uow.Setup(x => x.QuizAttempts).Returns(_attemptRepo.Object);
        _uow.Setup(x => x.QuizAnswers).Returns(_answerRepo.Object);
        _handler = new SubmitQuizHandler(_uow.Object, _publishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_QuizNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _quizRepo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Quiz, bool>>>())).ReturnsAsync((Quiz?)null);
        var command = new SubmitQuizCommand(Guid.NewGuid(), Guid.NewGuid(), []);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_MaxAttemptsReached_ThrowsInvalidOperationException()
    {
        // Arrange
        var quizId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var lectureId = Guid.NewGuid();
        var quiz = new Quiz { Id = quizId, LectureId = lectureId, MaxAttempts = 1 };
        
        _quizRepo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Quiz, bool>>>())).ReturnsAsync(quiz);
        _attemptRepo.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<QuizAttempt, bool>>>()))
                    .ReturnsAsync(true); // Already attempted

        var command = new SubmitQuizCommand(lectureId, studentId, []);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<InvalidOperationException>()
                      .WithMessage("*already submitted*");
    }

    [Fact]
    public async Task Handle_ValidMCQSubmission_AutoGradesCorrectly()
    {
        // Arrange
        var quizId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var optionId = Guid.NewGuid();

        var quiz = new Quiz 
        { 
            Id = quizId, 
            LectureId = Guid.NewGuid(),
            MaxAttempts = 2,
            Questions = new List<QuizQuestion>
            {
                new QuizQuestion 
                { 
                    Id = questionId, 
                    Type = QuestionType.MCQ,
                    Points = 10m,
                    Options = new List<QuizQuestionOption>
                    {
                        new QuizQuestionOption { Id = optionId, IsCorrect = true }
                    }
                }
            }
        };
        
        _quizRepo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Quiz, bool>>>())).ReturnsAsync(quiz);
        _attemptRepo.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<QuizAttempt, bool>>>()))
                    .ReturnsAsync(false);

        _attemptRepo.Setup(x => x.AddAsync(It.IsAny<QuizAttempt>())).Returns(Task.CompletedTask);
        _answerRepo.Setup(x => x.AddAsync(It.IsAny<QuizAnswer>())).Returns(Task.CompletedTask);
        _uow.Setup(x => x.QuizQuestions.GetByIdAsync(questionId)).ReturnsAsync(quiz.Questions.First());
        _uow.Setup(x => x.QuizQuestionOptions.GetByIdAsync(optionId)).ReturnsAsync(quiz.Questions.First().Options.First());
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish<IQuizCompleted>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var answers = new List<ExamService.Features.Quiz.SubmitQuiz.AnswerRequest> { new ExamService.Features.Quiz.SubmitQuiz.AnswerRequest(questionId, optionId, null) };
        var command = new SubmitQuizCommand(quiz.LectureId, studentId, answers);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Score.Should().Be(100m); // Auto-graded MCQ gave 100% (10/10)
        _attemptRepo.Verify(x => x.AddAsync(It.IsAny<QuizAttempt>()), Times.Once);
        _publishEndpoint.Verify(x => x.Publish<IQuizCompleted>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once); // Notifies quiz completion
    }
}

