using AuthService.Features.Auth.Login;
using FluentAssertions;

namespace AuthService.UnitTests.Validators;

public class LoginValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new LoginCommand { Username = "student@test.com", Password = "Password123!" };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyUsername_FailsValidation()
    {
        // Arrange
        var command = new LoginCommand { Username = "", Password = "Password123!" };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Username));
    }

    [Fact]
    public async Task Validate_EmptyPassword_FailsValidation()
    {
        // Arrange
        var command = new LoginCommand { Username = "student@test.com", Password = "" };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [Theory]
    [InlineData("", "Password123!")]
    [InlineData("student@test.com", "")]
    [InlineData("", "")]
    public async Task Validate_EmptyFields_FailsValidation(string username, string password)
    {
        // Arrange
        var command = new LoginCommand { Username = username, Password = password };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
