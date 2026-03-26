using FluentAssertions;
using GradeService.Features.Grades.SetMidtermGrade;

namespace GradeService.UnitTests.Validators;

public class SetMidtermGradeValidatorTests
{
    private readonly SetMidtermGradeValidator _validator = new();

    // ─── Valid command ────────────────────────────────────────────────────────

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), 75m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // ─── Score boundaries ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public async Task Validate_BoundaryScores_PassesValidation(decimal score)
    {
        // Arrange
        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), score);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // ─── Score out of range ───────────────────────────────────────────────────

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-100)]
    public async Task Validate_ScoreOutOfRange_FailsValidation(decimal score)
    {
        // Arrange
        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.NewGuid(), score);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetMidtermGradeCommand.Score));
    }

    // ─── Empty Ids ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Validate_EmptyCourseId_FailsValidation()
    {
        // Arrange
        var command = new SetMidtermGradeCommand(Guid.Empty, Guid.NewGuid(), 75m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetMidtermGradeCommand.CourseId));
    }

    [Fact]
    public async Task Validate_EmptyStudentId_FailsValidation()
    {
        // Arrange
        var command = new SetMidtermGradeCommand(Guid.NewGuid(), Guid.Empty, 75m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetMidtermGradeCommand.StudentId));
    }
}
