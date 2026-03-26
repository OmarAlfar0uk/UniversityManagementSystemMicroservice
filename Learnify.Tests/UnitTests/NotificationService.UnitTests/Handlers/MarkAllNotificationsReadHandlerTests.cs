
using FluentAssertions;
using Moq;
using NotificationService.Contracts;
using NotificationService.Data.Models;
using NotificationService.Features.Notifications.MarkAllNotificationsRead;

namespace NotificationService.UnitTests.Handlers;

public class MarkAllNotificationsReadHandlerTests
{
    private readonly Mock<IUnitOfWork>                       _uow    = new();
    private readonly Mock<IGenericRepository<Notification>> _repo   = new();
    private readonly MarkAllNotificationsReadHandler         _handler;

    public MarkAllNotificationsReadHandlerTests()
    {
        _uow.Setup(x => x.Notifications).Returns(_repo.Object);
        _handler = new MarkAllNotificationsReadHandler(_uow.Object);
    }

    [Fact]
    public async Task Handle_HasUnreadNotifications_MarksAllAsRead()
    {
        // Arrange
        var userId        = Guid.NewGuid();
        var unread        = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), RecipientId = userId, IsRead = false },
            new() { Id = Guid.NewGuid(), RecipientId = userId, IsRead = false },
        };

        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
             .ReturnsAsync(unread);
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var command = new MarkAllNotificationsReadCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        unread.Should().AllSatisfy(n => n.IsRead.Should().BeTrue());
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoUnreadNotifications_SavesWithoutError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
             .ReturnsAsync(new List<Notification>());
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var command = new MarkAllNotificationsReadCommand(userId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
