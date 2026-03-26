using FluentAssertions;
using Moq;
using NotificationService.Contracts;
using NotificationService.Data.Models;
using NotificationService.Features.Notifications.MarkNotificationRead;

namespace NotificationService.UnitTests.Handlers;

public class MarkNotificationReadHandlerTests
{
    private readonly Mock<IUnitOfWork>                       _uow    = new();
    private readonly Mock<IGenericRepository<Notification>> _repo   = new();
    private readonly MarkNotificationReadHandler             _handler;

    public MarkNotificationReadHandlerTests()
    {
        _uow.Setup(x => x.Notifications).Returns(_repo.Object);
        _handler = new MarkNotificationReadHandler(_uow.Object);
    }

    [Fact]
    public async Task Handle_ValidOwner_MarksNotificationAsRead()
    {
        // Arrange
        var userId         = Guid.NewGuid();
        var notificationId = Guid.NewGuid();
        var notification   = new Notification
        {
            Id          = notificationId,
            RecipientId = userId,
            IsRead      = false,
            Title       = "Test",
            Body        = "Test body"
        };

        _repo.Setup(x => x.GetByIdAsync(notificationId)).ReturnsAsync(notification);
        _repo.Setup(x => x.Update(It.IsAny<Notification>()));
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var command = new MarkNotificationReadCommand(notificationId, userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        notification.IsRead.Should().BeTrue();
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NotificationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Notification?)null);

        var command = new MarkNotificationReadCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_DifferentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notification   = new Notification
        {
            Id          = notificationId,
            RecipientId = Guid.NewGuid(), // owned by someone else
            IsRead      = false
        };

        _repo.Setup(x => x.GetByIdAsync(notificationId)).ReturnsAsync(notification);

        var command = new MarkNotificationReadCommand(notificationId, Guid.NewGuid()); // different user

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
