using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Features.Admin;
using ReportingDashboardService.Features.Doctor;
using ReportingDashboardService.Features.Parent;
using ReportingDashboardService.Features.Student;
using ReportingDashboardService.Middlewares;
using ReportingDashboardService.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// Serilog
// ─────────────────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/reporting-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// ─────────────────────────────────────────────────────────────────────────────
// Swagger
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Reporting Dashboard Service",
        Version = "v1",
        Description = "Aggregation reporting microservice — no database, pure HTTP aggregation"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat= "JWT",
        In          = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
// MediatR + FluentValidation
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ─────────────────────────────────────────────────────────────────────────────
// HttpContext accessor (required for JWT / Correlation-Id forwarding)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// ─────────────────────────────────────────────────────────────────────────────
// Named HTTP Clients
// ─────────────────────────────────────────────────────────────────────────────
string ResolveServiceUrl(string serviceName)
{
    var url =
        builder.Configuration[$"ServiceUrls:{serviceName}"] ??
        builder.Configuration[$"Services:{serviceName}"];

    if (string.IsNullOrWhiteSpace(url))
    {
        throw new InvalidOperationException(
            $"Missing URL for service '{serviceName}'. Expected configuration key 'ServiceUrls:{serviceName}' or 'Services:{serviceName}'.");
    }

    return url;
}

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(ResolveServiceUrl("Auth"));
    client.Timeout     = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("AcademicService", client =>
{
    client.BaseAddress = new Uri(ResolveServiceUrl("Academic"));
    client.Timeout     = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("AttendanceService", client =>
{
    client.BaseAddress = new Uri(ResolveServiceUrl("Attendance"));
    client.Timeout     = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("ExamService", client =>
{
    client.BaseAddress = new Uri(ResolveServiceUrl("Exam"));
    client.Timeout     = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("GradeService", client =>
{
    client.BaseAddress = new Uri(ResolveServiceUrl("Grade"));
    client.Timeout     = TimeSpan.FromSeconds(10);
});

// ─────────────────────────────────────────────────────────────────────────────
// Service Clients (DI)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthServiceClient,       AuthServiceClient>();
builder.Services.AddScoped<IAcademicServiceClient,   AcademicServiceClient>();
builder.Services.AddScoped<IAttendanceServiceClient, AttendanceServiceClient>();
builder.Services.AddScoped<IGradeServiceClient,      GradeServiceClient>();
builder.Services.AddScoped<IExamServiceClient,       ExamServiceClient>();

// ─────────────────────────────────────────────────────────────────────────────
// Memory Cache (for optional Admin dashboard caching)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();

// ─────────────────────────────────────────────────────────────────────────────
// JWT Authentication
// ─────────────────────────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey   = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings["Issuer"],
            ValidAudience            = jwtSettings["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// CORS
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─────────────────────────────────────────────────────────────────────────────
// Build
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//}
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reporting Dashboard Service v1"));

// ─────────────────────────────────────────────────────────────────────────────
// Middleware Pipeline
// ─────────────────────────────────────────────────────────────────────────────
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SerilogEnricherMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// Endpoint Registration
// ─────────────────────────────────────────────────────────────────────────────
app.MapStudentReportEndpoints();
app.MapParentReportEndpoints();
app.MapAdminReportEndpoints();
app.MapDoctorReportEndpoints();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    status  = "healthy",
    service = "ReportingDashboardService",
    utc     = DateTime.UtcNow
}));

app.Run();
