using FluentAssertions;
using GradeService.Contracts;
using GradeService.Data.Models;
using GradeService.Features.Grades.GetGPA;
using Moq;

namespace GradeService.UnitTests.Handlers;

public class GetGPAHandlerTests
{
    private readonly Mock<IUnitOfWork>                      _uow    = new();
    private readonly Mock<IGenericRepository<StudentGrade>> _repo   = new();
    private readonly GetGPAHandler                          _handler;

    public GetGPAHandlerTests()
    {
        _uow.Setup(x => x.StudentGrades).Returns(_repo.Object);
        _handler = new GetGPAHandler(_uow.Object);
    }

    // ─── No grades → GPA = 0 ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_NoGrades_ReturnsZeroGPA()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync(new List<StudentGrade>());

        var query = new GetGPAQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.GPA.Should().Be(0m);
        result.CoursesGraded.Should().Be(0);
    }

    // ─── Single perfect grade ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_PerfectScore_ReturnsFourPointGPA()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync(new List<StudentGrade>
             {
                 new() { StudentId = studentId, TotalScore = 95m }
             });

        var query = new GetGPAQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.GPA.Should().Be(4.0m);
        result.CoursesGraded.Should().Be(1);
    }

    // ─── Multiple courses averaged ────────────────────────────────────────────

    [Fact]
    public async Task Handle_MultipleCourses_ReturnsAveragedGPA()
    {
        // Arrange — 95 → 4.0, 75 → 3.5, 50 → 2.0  ⇒ avg = (4.0+3.5+2.0)/3 = 3.17
        var studentId = Guid.NewGuid();
        _repo.Setup(x => x.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StudentGrade, bool>>>()))
             .ReturnsAsync(new List<StudentGrade>
             {
                 new() { StudentId = studentId, TotalScore = 95m },
                 new() { StudentId = studentId, TotalScore = 75m },
                 new() { StudentId = studentId, TotalScore = 50m }
             });

        var query = new GetGPAQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.GPA.Should().BeApproximately(3.17m, 0.01m);
        result.CoursesGraded.Should().Be(3);
    }
}

