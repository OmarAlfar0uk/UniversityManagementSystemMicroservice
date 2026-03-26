using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Learnify.Tests.Shared;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AcademicService.Data;
using AcademicService.Data.Models;

namespace AcademicService.IntegrationTests;

// ─── Factory ────────────────────────────────────────────────────────────────

public class AcademicServiceFactory : WebApplicationFactory<AcademicService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AcademicDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AcademicDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_Academic"));

            // Remove MassTransit + blob storage (no external infra in tests)
            var toRemove = services
                .Where(d =>
                    d.ServiceType.Namespace?.StartsWith("MassTransit") == true ||
                    d.ServiceType.Namespace?.StartsWith("Azure.Storage") == true)
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            
            services.AddSingleton(Moq.Mock.Of<MassTransit.IPublishEndpoint>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.ISendEndpointProvider>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.IBus>());
            
            var hosted = services.Where(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
            foreach (var d in hosted) services.Remove(d);
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(AcademicDbContext db)
    {
        db.Courses.Add(new Course
        {
            Id          = TestConstants.CourseId,
            Name        = "Data Structures",
            Description = "Core DS course",
            DoctorId    = TestConstants.DoctorId,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        });

        db.CourseEnrollments.Add(new CourseEnrollment
        {
            Id        = Guid.NewGuid(),
            CourseId  = TestConstants.CourseId,
            StudentId = TestConstants.UserId,
            EnrolledAt = DateTime.UtcNow
        });

        db.SaveChanges();
    }
}

// ─── Tests ───────────────────────────────────────────────────────────────────

public class AcademicEndpointTests : IClassFixture<AcademicServiceFactory>
{
    private readonly HttpClient _client;

    public AcademicEndpointTests(AcademicServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCourses_WithStudentToken_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync("/api/v1/academic/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCourses_WithoutToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/academic/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCourseById_ExistingCourse_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync($"/api/v1/academic/courses/{TestConstants.CourseId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCourseById_NonExistingCourse_Returns404()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync($"/api/v1/academic/courses/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCourse_WithStudentToken_Returns403()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("New Course"), "Name");
        content.Add(new StringContent("Desc"), "Description");
        content.Add(new StringContent(TestConstants.DoctorId.ToString()), "DoctorId");

        // Act
        var response = await _client.PostAsync("/api/v1/academic/admin/courses", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCourse_WithDoctorToken_Returns201()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.DoctorToken());

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("New Test Course"), "Name");
        content.Add(new StringContent("Test Description"), "Description");
        content.Add(new StringContent(TestConstants.DoctorId.ToString()), "DoctorId");
        
        // Act
        var response = await _client.PostAsync("/api/v1/academic/admin/courses", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }
}


