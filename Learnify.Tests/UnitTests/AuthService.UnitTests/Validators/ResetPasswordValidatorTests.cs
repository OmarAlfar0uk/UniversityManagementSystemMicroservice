using AuthService.Features.Auth.ResetPassword;
using FluentAssertions;

namespace AuthService.UnitTests.Validators;

public class ResetPasswordValidatorTests
{
    private readonly ResetPasswordValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            EmailOrId  = "student@test.com",
            ResetToken = "valid-token",
            NewPassword = "NewPassword123!"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyEmailOrId_FailsValidation()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            EmailOrId   = "",
            ResetToken  = "token",
            NewPassword = "Password123!"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ResetPasswordCommand.EmailOrId));
    }

    [Fact]
    public async Task Validate_EmptyResetToken_FailsValidation()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            EmailOrId   = "student@test.com",
            ResetToken  = "",
            NewPassword = "Password123!"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ResetPasswordCommand.ResetToken));
    }

    [Fact]
    public async Task Validate_ShortPassword_FailsValidation()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            EmailOrId   = "student@test.com",
            ResetToken  = "token",
            NewPassword = "123"  // too short
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ResetPasswordCommand.NewPassword));
    }
}
