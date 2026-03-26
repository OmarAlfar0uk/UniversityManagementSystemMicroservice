using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Features.Quiz.SubmitQuiz;
using FluentAssertions;
using MassTransit;
using Moq;

namespace ExamService.UnitTests.Validators;

public class SubmitQuizValidatorTests
{
    private readonly SubmitQuizValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new SubmitQuizCommand(
            Guid.NewGuid(), Guid.NewGuid(),
            new List<AnswerRequest> { new(Guid.NewGuid(), Guid.NewGuid(), null) });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAnswers_FailsValidation()
    {
        // Arrange
        var command = new SubmitQuizCommand(Guid.NewGuid(), Guid.NewGuid(), new List<AnswerRequest>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitQuizCommand.Answers));
    }

    [Fact]
    public async Task Validate_EmptyLectureId_FailsValidation()
    {
        // Arrange
        var command = new SubmitQuizCommand(
            Guid.Empty, Guid.NewGuid(),
            new List<AnswerRequest> { new(Guid.NewGuid(), null, "Essay answer") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitQuizCommand.LectureId));
    }

    [Fact]
    public async Task Validate_EmptyStudentId_FailsValidation()
    {
        // Arrange
        var command = new SubmitQuizCommand(
            Guid.NewGuid(), Guid.Empty,
            new List<AnswerRequest> { new(Guid.NewGuid(), null, "Answer") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SubmitQuizCommand.StudentId));
    }
}
