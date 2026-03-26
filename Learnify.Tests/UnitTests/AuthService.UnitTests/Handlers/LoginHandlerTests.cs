using AuthService.Contracts;
using AuthService.Models;
using AuthService.Features.Auth.Login;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Auth.Contarcts;
using Auth.Models;

namespace AuthService.UnitTests.Handlers;

public class LoginHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
            
        _handler = new LoginHandler(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "test@test.com", Email = "test@test.com", IsActivated = true };
        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password")).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Student" });
        _tokenServiceMock.Setup(x => x.GenerateTokensAsync(user, It.IsAny<bool>()))
                         .ReturnsAsync(("valid.jwt.token", "valid.refresh.token"));

        var command = new LoginCommand { Username = "test@test.com", Password = "password" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Data.AccessToken.Should().Be("valid.jwt.token");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        var command = new LoginCommand { Username = "wrong@test.com", Password = "password" };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorized()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "test@test.com", IsActivated = true };
        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);
        
        var command = new LoginCommand { Username = "test@test.com", Password = "wrongpassword" };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_InactiveUser_ThrowsUnauthorized()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "test@test.com", IsActivated = false }; // inactive
        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        
        var command = new LoginCommand { Username = "test@test.com", Password = "password" };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<UnauthorizedAccessException>()
                      .WithMessage("*inactive*");
    }
}
