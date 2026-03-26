using FluentAssertions;
using Moq;
using NotificationService.Contracts;
using NotificationService.Data.Models;
using NotificationService.Features.Notifications.GetNotifications;

namespace NotificationService.UnitTests.Handlers;

public class GetNotificationsHandlerTests
{
    private readonly Mock<IUnitOfWork>                       _uow    = new();
    private readonly Mock<IGenericRepository<Notification>> _repo   = new();
    private readonly GetNotificationsHandler                 _handler;

    public GetNotificationsHandlerTests()
    {
        _uow.Setup(x => x.Notifications).Returns(_repo.Object);
        _handler = new GetNotificationsHandler(_uow.Object);
    }

    [Fact]
    public async Task Handle_HasNotifications_ReturnsPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notifications = Enumerable.Range(1, 15).Select(i => new Notification
        {
            Id          = Guid.NewGuid(),
            RecipientId = userId,
            Title       = $"Notification {i}",
            Body        = "Body",
            IsRead      = false,
            CreatedAt   = DateTime.UtcNow.AddMinutes(-i)
        }).ToList();

        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
             .ReturnsAsync(notifications);

        var query = new GetNotificationsQuery(userId, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.TotalCount.Should().Be(15);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_NoNotifications_ReturnsEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
             .ReturnsAsync(new List<Notification>());

        var query = new GetNotificationsQuery(userId, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Notifications_ReturnsMostRecentFirst()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var older  = new Notification { Id = Guid.NewGuid(), RecipientId = userId, Title = "Old", Body = "", CreatedAt = DateTime.UtcNow.AddHours(-2) };
        var newer  = new Notification { Id = Guid.NewGuid(), RecipientId = userId, Title = "New", Body = "", CreatedAt = DateTime.UtcNow };

        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>()))
             .ReturnsAsync(new List<Notification> { older, newer });

        var query = new GetNotificationsQuery(userId, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.First().Title.Should().Be("New");
    }
}
