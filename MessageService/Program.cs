using FluentValidation;
using MediatR;
using MessageService.Features.AI;
using MessageService.Features.Messages;
using MessageService.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message Service", Version = "v1" });

    // Add security definition for JWT Bearer tokens
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid JWT token below.\r\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
    });
    
    // Make sure endpoints require this security definition
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MessageService.Data.MessageDbContext>(options =>
    Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));

builder.Services.AddScoped<MessageService.Contracts.IUnitOfWork, MessageService.Repositories.UnitOfWork>();
builder.Services.AddScoped<MessageService.Contracts.IImageHelper, MessageService.Services.ImageHelper>();
builder.Services.AddScoped<MessageService.Contracts.IFileHelper, MessageService.Services.FileHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AuthService"] ?? "http://localhost:8081");
});
builder.Services.AddScoped<MessageService.Contracts.IAuthServiceClient, MessageService.Services.AuthServiceClient>();
builder.Services.AddHttpClient("Gemini", client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
});
builder.Services
    .AddOptions<AiSettings>()
    .Bind(builder.Configuration.GetSection("AiSettings"))
    .PostConfigure(settings =>
    {
        settings.ApiKey = builder.Configuration["GeminiSettings:ApiKey"]
            ?? builder.Configuration["GEMINI_API_KEY"]
            ?? settings.ApiKey;
    })
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.ApiKey),
        "Gemini API key is missing. Set GeminiSettings__ApiKey or GEMINI_API_KEY.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.Model),
        "Gemini model is missing. Set AiSettings__Model.")
    .ValidateOnStart();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return false;

                return uri.Host is "localhost" or "127.0.0.1" ||
                       uri.Host is "learnify.tech" or "www.learnify.tech" or "learnefy.tech" or "www.learnefy.tech" ||
                       uri.Host.EndsWith(".learnefy.tech", StringComparison.OrdinalIgnoreCase) ||
                       uri.Host.EndsWith(".learnify.tech", StringComparison.OrdinalIgnoreCase) ||
                       uri.Host.Equals("learnify-jqme.vercel.app", StringComparison.OrdinalIgnoreCase);
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// MediatR DI
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Logging
builder.Logging.AddConsole();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//}
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Service v1");
    });

app.UseMiddleware<MessageService.Middlewares.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapAiEndpoints();
app.MapMessageEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{   
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
