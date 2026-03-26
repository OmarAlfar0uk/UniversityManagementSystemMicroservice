using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth.Models;
using AuthService.Data;
using FluentAssertions;
using Learnify.Tests.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.IntegrationTests;

public class AuthServiceFactory : WebApplicationFactory<Auth_Service.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UniversitySystemAuthContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<UniversitySystemAuthContext>(options =>
                options.UseInMemoryDatabase("TestDb_Auth_" + Guid.NewGuid()));
                
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
            var db = scope.ServiceProvider.GetRequiredService<UniversitySystemAuthContext>();
            db.Database.EnsureCreated();
            SeedTestData(scope.ServiceProvider).GetAwaiter().GetResult();
        });
    }

    private static async Task SeedTestData(IServiceProvider sp)
    {
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<ApplicationRole>>();

        await roleManager.CreateAsync(new ApplicationRole { Name = "Student" });

        var user = new ApplicationUser
        {
            Id = TestConstants.UserId,
            UserName = "student@test.com",
            Email = "student@test.com",
            FirstName = "Test",
            LastName = "Student",
            UniversityId = "U12345",
            Gender = Gender.Male,
            IsActivated = true
        };

        await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, "Student");
    }
}

public class AuthEndpointTests : IClassFixture<AuthServiceFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(AuthServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200AndToken()
    {
        // Arrange
        var payload = new { Username = "student@test.com", Password = "Password123!" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("token");
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        // Arrange
        var payload = new { Username = "student@test.com", Password = "WrongPassword!" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetUsers_WithStudentToken_Returns403()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtHelper.StudentToken());

        // Act
        var response = await _client.GetAsync("/api/v1/auth/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

