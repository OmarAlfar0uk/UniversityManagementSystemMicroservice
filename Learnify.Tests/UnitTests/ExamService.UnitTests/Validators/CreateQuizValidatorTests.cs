using ExamService.Features.Quiz.CreateQuiz;
using FluentAssertions;

namespace ExamService.UnitTests.Validators;

public class CreateQuizValidatorTests
{
    private readonly CreateQuizValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateQuizCommand(Guid.NewGuid(), Guid.NewGuid(), 30, 1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroDuration_FailsValidation()
    {
        // Arrange
        var command = new CreateQuizCommand(Guid.NewGuid(), Guid.NewGuid(), 0, 1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateQuizCommand.TimeLimitInMinutes));
    }

    [Fact]
    public async Task Validate_ZeroMaxAttempts_FailsValidation()
    {
        // Arrange
        var command = new CreateQuizCommand(Guid.NewGuid(), Guid.NewGuid(), 30, 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateQuizCommand.MaxAttempts));
    }

    [Fact]
    public async Task Validate_EmptyLectureId_FailsValidation()
    {
        // Arrange
        var command = new CreateQuizCommand(Guid.Empty, Guid.NewGuid(), 30, 1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateQuizCommand.LectureId));
    }
}
