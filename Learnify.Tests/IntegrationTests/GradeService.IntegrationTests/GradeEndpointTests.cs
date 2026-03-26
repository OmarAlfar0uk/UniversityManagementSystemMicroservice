using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Learnify.Tests.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GradeService.Data;
using GradeService.Data.Models;
using MassTransit;

namespace GradeService.IntegrationTests;

// ─── Factory ────────────────────────────────────────────────────────────────

public class GradeServiceFactory : WebApplicationFactory<GradeService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GradeDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<GradeDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_Grade_" + Guid.NewGuid()));

            // Remove MassTransit (no RabbitMQ in tests)
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
            var db = scope.ServiceProvider.GetRequiredService<GradeDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(GradeDbContext db)
    {
        db.StudentGrades.Add(new StudentGrade
        {
            Id           = Guid.NewGuid(),
            CourseId     = TestConstants.CourseId,
            StudentId    = TestConstants.UserId,
            MidtermScore = 80m,
            FinalScore   = 75m,
            TotalScore   = 80m,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

// ─── Tests ───────────────────────────────────────────────────────────────────

public class GradeEndpointTests : IClassFixture<GradeServiceFactory>
{
    private readonly HttpClient _client;

    public GradeEndpointTests(GradeServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetGPA_WithStudentToken_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync("/api/v1/grade/gpa");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGPA_WithoutToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/grade/gpa");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SetMidtermGrade_WithDoctorToken_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.DoctorToken());

        var payload = new { StudentId = TestConstants.UserId, Score = 85 };
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(
            $"/api/v1/grade/admin/courses/{TestConstants.CourseId}/students/{TestConstants.UserId}/midterm",
            content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
    }

    [Fact]
    public async Task SetMidtermGrade_WithStudentToken_Returns403()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        var payload = new { Score = 85 };
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(
            $"/api/v1/grade/admin/courses/{TestConstants.CourseId}/students/{TestConstants.UserId}/midterm",
            content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}


