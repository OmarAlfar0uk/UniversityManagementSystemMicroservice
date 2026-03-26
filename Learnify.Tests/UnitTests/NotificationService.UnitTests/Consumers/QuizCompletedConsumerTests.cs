using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using NotificationService.Consumers;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.UnitTests.Consumers;

public class QuizCompletedConsumerTests
{
    private readonly Mock<IMediator>          _mediator = new();
    private readonly QuizCompletedConsumer    _consumer;

    public QuizCompletedConsumerTests()
    {
        _consumer = new QuizCompletedConsumer(_mediator.Object);
    }

    [Fact]
    public async Task Consume_PassedQuiz_BodyContainsPassedStatus()
    {
        // Arrange
        SendNotificationCommand? captured = null;
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<IQuizCompleted>();
        message.Setup(m => m.StudentId).Returns(Guid.NewGuid());
        message.Setup(m => m.Score).Returns(85m);
        message.Setup(m => m.IsPassed).Returns(true);
        message.Setup(m => m.CompletedAt).Returns(DateTime.UtcNow);

        var context = new Mock<ConsumeContext<IQuizCompleted>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.Body.Should().Contain("85");
        captured.Body.Should().Contain("Passed");
    }

    [Fact]
    public async Task Consume_FailedQuiz_BodyContainsFailedStatus()
    {
        // Arrange
        SendNotificationCommand? captured = null;
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<IQuizCompleted>();
        message.Setup(m => m.StudentId).Returns(Guid.NewGuid());
        message.Setup(m => m.Score).Returns(40m);
        message.Setup(m => m.IsPassed).Returns(false);
        message.Setup(m => m.CompletedAt).Returns(DateTime.UtcNow);

        var context = new Mock<ConsumeContext<IQuizCompleted>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.Body.Should().Contain("40");
        captured.Body.Should().Contain("Failed");
    }
}

