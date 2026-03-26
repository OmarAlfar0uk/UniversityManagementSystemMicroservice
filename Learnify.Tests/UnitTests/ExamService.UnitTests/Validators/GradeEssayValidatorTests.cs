using ExamService.Features.Quiz.GradeEssay;
using FluentAssertions;

namespace ExamService.UnitTests.Validators;

public class GradeEssayValidatorTests
{
    private readonly GradeEssayValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new GradeEssayCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5.0m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroPoints_PassesValidation()
    {
        // Arrange — 0 is valid (student answered incorrectly)
        var command = new GradeEssayCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NegativePoints_FailsValidation()
    {
        // Arrange
        var command = new GradeEssayCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -1m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GradeEssayCommand.EarnedPoints));
    }

    [Fact]
    public async Task Validate_EmptyQuizId_FailsValidation()
    {
        // Arrange
        var command = new GradeEssayCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 5m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GradeEssayCommand.QuizId));
    }
}
