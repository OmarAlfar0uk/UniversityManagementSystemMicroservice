using FluentAssertions;
using GradeService.Contracts;
using GradeService.Data.Models;
using GradeService.Features.Grades.SetMidtermGrade;
using MassTransit;
using Moq;

namespace GradeService.UnitTests.Handlers;

public class SetMidtermGradeHandlerTests
{
    private readonly Mock<IUnitOfWork>         _uow            = new();
    private readonly Mock<IGenericRepository<StudentGrade>> _repo = new();
    private readonly Mock<IPublishEndpoint>    _publishEndpoint = new();
    private readonly SetMidtermGradeHandler    _handler;

    public SetMidtermGradeHandlerTests()
    {
        _uow.Setup(x => x.StudentGrades).Returns(_repo.Object);
        _handler = new SetMidtermGradeHandler(_uow.Object, _publishEndpoint.Object);
    }

    // ─── Create new grade ───────────────────────────────────────────────────

    [Fact]
    public async Task Handle_NoExistingGrade_CreatesNewGradeRecord()
    {
        // Arrange
        _repo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync((StudentGrade?)null);
        _repo.Setup(x => x.AddAsync(It.IsAny<StudentGrade>())).Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), 75m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(75m);
        _repo.Verify(x => x.AddAsync(It.IsAny<StudentGrade>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    // ─── Update existing grade ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_ExistingGrade_UpdatesMidtermScore()
    {
        // Arrange
        var existing = new StudentGrade
        {
            Id             = Guid.NewGuid(),
            CourseId       = Guid.NewGuid(),
            StudentId      = Guid.NewGuid(),
            MidtermScore   = 50m,
            AttendanceScore = 10m, AssignmentScore = 10m,
            QuizScore = 10m, FinalScore = 0m
        };
        _repo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync(existing);
        _repo.Setup(x => x.Update(It.IsAny<StudentGrade>()));
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new SetMidtermGradeCommand(existing.CourseId, existing.StudentId, 90m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Score.Should().Be(90m);
        _repo.Verify(x => x.Update(It.IsAny<StudentGrade>()), Times.Once);
        _repo.Verify(x => x.AddAsync(It.IsAny<StudentGrade>()), Times.Never);
    }

    // ─── Correct letter grade ────────────────────────────────────────────────

    [Theory]
    [InlineData(95, "A+")]
    [InlineData(85, "A")]
    [InlineData(50, "D")]
    [InlineData(30, "F")]
    public async Task Handle_ScoreRange_ReturnsCorrectLetter(int score, string expectedLetter)
    {
        // Arrange
        _repo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync((StudentGrade?)null);
        _repo.Setup(x => x.AddAsync(It.IsAny<StudentGrade>())).Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), (decimal)score);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.LetterGrade.Should().Be(expectedLetter);
    }

    // ─── Event published ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidGrade_PublishesIGradeAddedEvent()
    {
        // Arrange
        _repo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync((StudentGrade?)null);
        _repo.Setup(x => x.AddAsync(It.IsAny<StudentGrade>())).Returns(Task.CompletedTask);
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), 80m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publishEndpoint.Verify(
            x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
