using FluentAssertions;
using GradeService.Features.Grades.SetFinalGrade;

namespace GradeService.UnitTests.Validators;

public class SetFinalGradeValidatorTests
{
    private readonly SetFinalGradeValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new SetFinalGradeCommand(Guid.NewGuid(), Guid.NewGuid(), 85m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Validate_ScoreOutOfRange_FailsValidation(decimal score)
    {
        // Arrange
        var command = new SetFinalGradeCommand(Guid.NewGuid(), Guid.NewGuid(), score);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetFinalGradeCommand.Score));
    }

    [Fact]
    public async Task Validate_EmptyCourseId_FailsValidation()
    {
        // Arrange
        var command = new SetFinalGradeCommand(Guid.Empty, Guid.NewGuid(), 85m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
