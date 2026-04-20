using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Features.Auth.Admin.DeleteUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.UnitTests.Handlers;

public class DeleteUserHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IAuthAuditLogger> _auditLoggerMock = new();
    private readonly Mock<IAcademicServiceClient> _academicServiceClientMock = new();
    private readonly UniversitySystemAuthContext _context;

    public DeleteUserHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        var dbOptions = new DbContextOptionsBuilder<UniversitySystemAuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new UniversitySystemAuthContext(dbOptions);
    }

    [Fact]
    public async Task Handle_WhenDeletingStudent_CleansAcademicEnrollments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "student@test.com",
            Email = "student@test.com"
        };

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Student" });
        _userManagerMock
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _academicServiceClientMock
            .Setup(x => x.DeleteStudentEnrollmentsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _auditLoggerMock
            .Setup(x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteUserHandler(
            _userManagerMock.Object,
            _auditLoggerMock.Object,
            _academicServiceClientMock.Object,
            _context);

        // Act
        var result = await handler.Handle(
            new DeleteUserCommand { UserId = userId },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("User deleted successfully");
        result.Message.Should().Be("User deleted successfully.");

        _academicServiceClientMock.Verify(
            x => x.DeleteStudentEnrollmentsAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
