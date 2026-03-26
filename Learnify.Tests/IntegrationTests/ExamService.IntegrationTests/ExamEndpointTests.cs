using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Learnify.Tests.Shared;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExamService.Data;
using ExamService.Data.Models;
using ExamService.Data.Enums;

namespace ExamService.IntegrationTests;

// ─── Factory ────────────────────────────────────────────────────────────────

public class ExamServiceFactory : WebApplicationFactory<ExamService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ExamDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<ExamDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_Exam_" + Guid.NewGuid()));

            // Remove MassTransit
            var mtDescriptors = services
                .Where(d => d.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                .ToList();
            foreach (var d in mtDescriptors) services.Remove(d);

            
            services.AddSingleton(Moq.Mock.Of<MassTransit.IPublishEndpoint>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.ISendEndpointProvider>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.IBus>());
            
            var hosted = services.Where(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
            foreach (var d in hosted) services.Remove(d);
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ExamDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(ExamDbContext db)
    {
        var quiz = new Quiz
        {
            Id                = TestConstants.QuizId,
            LectureId         = TestConstants.LectureId,
            CourseId          = TestConstants.CourseId,
            TimeLimitInMinutes = 30,
            MaxAttempts       = 1,
            IsActive          = true,
            CreatedAt         = DateTime.UtcNow,
            UpdatedAt         = DateTime.UtcNow
        };

        db.Quizzes.Add(quiz);
        db.SaveChanges();
    }
}

// ─── Tests ───────────────────────────────────────────────────────────────────

public class ExamEndpointTests : IClassFixture<ExamServiceFactory>
{
    private readonly HttpClient _client;

    public ExamEndpointTests(ExamServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetQuizStatus_ExistingLecture_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync($"/api/v1/exam/lectures/{TestConstants.LectureId}/quiz/status");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuizStatus_NonExistingLecture_Returns404()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync($"/api/v1/exam/lectures/{Guid.NewGuid()}/quiz/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuizStatus_WithoutToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/api/v1/exam/lectures/{TestConstants.LectureId}/quiz/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GradeEssay_WithStudentToken_Returns403()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        var payload = new { EarnedPoints = 5m };
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(
            $"/api/v1/exam/admin/quizzes/{TestConstants.QuizId}/attempts/{Guid.NewGuid()}/questions/{Guid.NewGuid()}/grade",
            content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}


