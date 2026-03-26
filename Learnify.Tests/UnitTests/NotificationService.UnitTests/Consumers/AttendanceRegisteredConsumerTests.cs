using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using NotificationService.Consumers;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.UnitTests.Consumers;

public class AttendanceRegisteredConsumerTests
{
    private readonly Mock<IMediator>                   _mediator = new();
    private readonly AttendanceRegisteredConsumer      _consumer;

    public AttendanceRegisteredConsumerTests()
    {
        _consumer = new AttendanceRegisteredConsumer(_mediator.Object);
    }

    [Fact]
    public async Task Consume_ValidEvent_SendsNotificationToCorrectStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        SendNotificationCommand? captured = null;

        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<IAttendanceRegistered>();
        message.Setup(m => m.StudentId).Returns(studentId);
        message.Setup(m => m.CourseId).Returns(Guid.NewGuid());
        message.Setup(m => m.LectureId).Returns(Guid.NewGuid());
        message.Setup(m => m.Date).Returns(DateTime.UtcNow);

        var context = new Mock<ConsumeContext<IAttendanceRegistered>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _mediator.Verify(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        captured!.UserId.Should().Be(studentId);
    }

    [Fact]
    public async Task Consume_ValidEvent_BodyMentionsAttendance()
    {
        // Arrange
        var message = new Mock<IAttendanceRegistered>();
        message.Setup(m => m.StudentId).Returns(Guid.NewGuid());
        message.Setup(m => m.Date).Returns(new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        SendNotificationCommand? captured = null;
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var context = new Mock<ConsumeContext<IAttendanceRegistered>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.Title.Should().ContainEquivalentOf("Attendance");
    }
}

