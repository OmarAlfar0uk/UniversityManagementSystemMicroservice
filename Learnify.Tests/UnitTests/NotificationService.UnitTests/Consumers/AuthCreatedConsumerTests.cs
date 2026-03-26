using FluentAssertions;
using MassTransit;
using MediatR;
using NotificationService.Consumers;
using NotificationService.Contracts;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.UnitTests.Consumers;

public class AuthCreatedConsumerTests
{
    private readonly Mock<IMediator>                    _mediator     = new();
    private readonly Mock<INotificationAuditLogger>     _auditLogger  = new();
    private readonly AuthCreatedConsumer                _consumer;

    public AuthCreatedConsumerTests()
    {
        _consumer = new AuthCreatedConsumer(_mediator.Object, _auditLogger.Object);
    }

    [Fact]
    public async Task Consume_ValidEvent_SendsSendNotificationCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SendNotificationCommand? capturedCommand = null;

        _auditLogger.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) =>
                     capturedCommand = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var context = new Mock<ConsumeContext<IAuthCreated>>();
        var message = new Mock<IAuthCreated>();
        message.Setup(m => m.UserId).Returns(userId);
        message.Setup(m => m.Email).Returns("test@test.com");
        message.Setup(m => m.UserName).Returns("John Doe");
        message.Setup(m => m.Role).Returns("Student");
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _mediator.Verify(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        capturedCommand.Should().NotBeNull();
        capturedCommand!.UserId.Should().Be(userId);
        capturedCommand.Title.Should().NotBeNullOrEmpty();
    }
}

