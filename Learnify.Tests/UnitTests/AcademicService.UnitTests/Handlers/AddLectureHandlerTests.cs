using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.Courses;
using Moq;
using FluentAssertions;
using MassTransit;
using AcademicService.Features.Lectures.AddLecture;
using Shered.Events;

namespace AcademicService.UnitTests.Handlers;

public class AddLectureHandlerTests
{
    private readonly Mock<IUnitOfWork>               _uow             = new();
    private readonly Mock<IGenericRepository<Course>> _courseRepo     = new();
    private readonly Mock<IGenericRepository<Lecture>> _lectureRepo   = new();
    private readonly Mock<IGenericRepository<CourseEnrollment>> _enrollmentRepo = new();
    private readonly Mock<IPublishEndpoint>          _publishEndpoint = new();
    private readonly Mock<IImageHelper>              _imageHelper     = new();
    private readonly AddLectureHandler               _handler;

    public AddLectureHandlerTests()
    {
        _uow.Setup(x => x.Courses).Returns(_courseRepo.Object);
        _uow.Setup(x => x.Lectures).Returns(_lectureRepo.Object);
        _uow.Setup(x => x.CourseEnrollments).Returns(_enrollmentRepo.Object);
        _handler = new AddLectureHandler(_uow.Object, _imageHelper.Object, _publishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_CourseNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _courseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Course?)null);
        var command = new AddLectureCommand(Guid.NewGuid(), "New Lecture", 1, null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                      .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesLectureAndPublishesEvent()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course 
        { 
            Id = courseId, 
            Name = "Math", 
            Enrollments = new List<CourseEnrollment> 
            { 
                new CourseEnrollment { StudentId = Guid.NewGuid() } 
            } 
        };

        _courseRepo.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _lectureRepo.Setup(x => x.AddAsync(It.IsAny<Lecture>())).Returns(Task.CompletedTask);
        _enrollmentRepo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<CourseEnrollment, bool>>>()))
                        .ReturnsAsync(course.Enrollments);
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _publishEndpoint.Setup(x => x.Publish<ILectureAdded>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new AddLectureCommand(courseId, "Lecture 1", 1, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Lecture 1");
        
        _lectureRepo.Verify(x => x.AddAsync(It.IsAny<Lecture>()), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
        _publishEndpoint.Verify(x => x.Publish<ILectureAdded>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once); // Publishes ILectureAdded
    }
}
