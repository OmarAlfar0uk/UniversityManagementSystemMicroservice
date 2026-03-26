using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using NotificationService.Consumers;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.UnitTests.Consumers;

public class GradeAddedConsumerTests
{
    private readonly Mock<IMediator>       _mediator = new();
    private readonly GradeAddedConsumer    _consumer;

    public GradeAddedConsumerTests()
    {
        _consumer = new GradeAddedConsumer(_mediator.Object);
    }

    [Theory]
    [InlineData("Midterm")]
    [InlineData("Final")]
    public async Task Consume_GradeAdded_BodyContainsGradeType(string gradeType)
    {
        // Arrange
        SendNotificationCommand? captured = null;
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<IGradeAdded>();
        message.Setup(m => m.StudentId).Returns(Guid.NewGuid());
        message.Setup(m => m.CourseId).Returns(Guid.NewGuid());
        message.Setup(m => m.GradeType).Returns(gradeType);
        message.Setup(m => m.Score).Returns(80m);

        var context = new Mock<ConsumeContext<IGradeAdded>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.Title.Should().Contain(gradeType);
        captured.Body.Should().Contain("80");
    }

    [Fact]
    public async Task Consume_GradeAdded_NotificationSentToCorrectStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        SendNotificationCommand? captured = null;

        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<IGradeAdded>();
        message.Setup(m => m.StudentId).Returns(studentId);
        message.Setup(m => m.GradeType).Returns("Final");
        message.Setup(m => m.Score).Returns(90m);

        var context = new Mock<ConsumeContext<IGradeAdded>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.UserId.Should().Be(studentId);
    }
}

