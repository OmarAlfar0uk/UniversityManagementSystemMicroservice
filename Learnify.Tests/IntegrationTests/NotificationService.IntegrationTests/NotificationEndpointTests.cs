using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Learnify.Tests.Shared;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Contracts;
using NotificationService.Data;
using NotificationService.Data.Models;
using NotificationService.Repositories;

namespace NotificationService.IntegrationTests;

// ─── Factory ────────────────────────────────────────────────────────────────

public class NotificationServiceFactory : WebApplicationFactory<NotificationService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server with InMemory
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<NotificationDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<NotificationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_Notification_" + Guid.NewGuid()));

            // Remove MassTransit to avoid RabbitMQ dependency in tests
            var massTransitDescriptors = services
                .Where(d => d.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                .ToList();
            foreach (var d in massTransitDescriptors) services.Remove(d);

            // Seed test data
            
            services.AddSingleton(Moq.Mock.Of<MassTransit.IPublishEndpoint>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.ISendEndpointProvider>());
            services.AddSingleton(Moq.Mock.Of<MassTransit.IBus>());
            
            var hosted = services.Where(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
            foreach (var d in hosted) services.Remove(d);
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(NotificationDbContext db)
    {
        db.Notifications.Add(new Notification
        {
            Id          = TestConstants.NotifId,
            RecipientId = TestConstants.UserId,
            Title       = "Test Notification",
            Body        = "Test body",
            IsRead      = false,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

// ─── Tests ───────────────────────────────────────────────────────────────────

public class NotificationEndpointTests : IClassFixture<NotificationServiceFactory>
{
    private readonly HttpClient _client;

    public NotificationEndpointTests(NotificationServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetNotifications_WithValidToken_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync("/api/v1/notification?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetNotifications_WithoutToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/notification?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MarkAllRead_WithValidToken_Returns200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.PutAsync("/api/v1/notification/read-all", null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MarkRead_NotExistingNotification_Returns404()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.PutAsync($"/api/v1/notification/{Guid.NewGuid()}/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NotExistingNotification_Returns404()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.DeleteAsync($"/api/v1/notification/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}


