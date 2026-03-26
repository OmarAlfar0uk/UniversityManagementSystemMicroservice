using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;
using NotificationService.Consumers;
using NotificationService.Features.Notifications.SendNotification;
using Shered.Events;

namespace NotificationService.UnitTests.Consumers;

public class LectureAddedConsumerTests
{
    private readonly Mock<IMediator>          _mediator = new();
    private readonly LectureAddedConsumer     _consumer;

    public LectureAddedConsumerTests()
    {
        _consumer = new LectureAddedConsumer(_mediator.Object);
    }

    [Fact]
    public async Task Consume_ThreeStudents_SendsThreeNotifications()
    {
        // Arrange
        var studentIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var sentCommands = new List<SendNotificationCommand>();

        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) =>
                 {
                     if (cmd is SendNotificationCommand snc) sentCommands.Add(snc);
                 })
                 .Returns(Task.CompletedTask);

        var message = new Mock<ILectureAdded>();
        message.Setup(m => m.LectureId).Returns(Guid.NewGuid());
        message.Setup(m => m.CourseId).Returns(Guid.NewGuid());
        message.Setup(m => m.CourseName).Returns("Data Structures");
        message.Setup(m => m.LectureTitle).Returns("Lecture 1");
        message.Setup(m => m.StudentIds).Returns(studentIds.ToList().AsReadOnly());

        var context = new Mock<ConsumeContext<ILectureAdded>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        sentCommands.Should().HaveCount(3);
        sentCommands.Select(c => c.UserId).Should().BeEquivalentTo(studentIds);
    }

    [Fact]
    public async Task Consume_LectureAdded_NotificationTitleContainsLectureName()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        SendNotificationCommand? captured = null;

        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Callback<IRequest, CancellationToken>((cmd, _) => captured = cmd as SendNotificationCommand)
                 .Returns(Task.CompletedTask);

        var message = new Mock<ILectureAdded>();
        message.Setup(m => m.LectureTitle).Returns("Trees & Graphs");
        message.Setup(m => m.CourseName).Returns("DSA");
        message.Setup(m => m.StudentIds).Returns(new List<Guid> { studentId }.AsReadOnly());

        var context = new Mock<ConsumeContext<ILectureAdded>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        captured!.Title.Should().Contain("Lecture");
        captured.Body.Should().Contain("Trees & Graphs");
    }

    [Fact]
    public async Task Consume_NoStudents_SendsZeroNotifications()
    {
        // Arrange
        _mediator.Setup(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var message = new Mock<ILectureAdded>();
        message.Setup(m => m.LectureTitle).Returns("Intro");
        message.Setup(m => m.CourseName).Returns("CS101");
        message.Setup(m => m.StudentIds).Returns(new List<Guid>().AsReadOnly());

        var context = new Mock<ConsumeContext<ILectureAdded>>();
        context.Setup(c => c.Message).Returns(message.Object);
        context.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(context.Object);

        // Assert
        _mediator.Verify(x => x.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

