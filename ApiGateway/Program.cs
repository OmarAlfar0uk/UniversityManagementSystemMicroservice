using System.Security.Claims;
using System.Text;
using ApiGateway.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 6. Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// 1. Load YARP config
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 2. JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException(
                    "JwtSettings are missing. Please set JwtSettings:Issuer, JwtSettings:Audience, and JwtSettings:SecretKey.");
            }

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// 3. Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
});

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromSeconds(60);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new { message = "Too many requests. Please try again later." }, token);
    };
});

// 7. HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 10. Swagger Configuration for Aggregation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Learnify API Gateway", Version = "v1" });
});

var app = builder.Build();

// Pipeline Order
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/v1/swagger.json", "API Gateway");
        c.SwaggerEndpoint("http://localhost:5000/swagger/v1/swagger.json", "Auth Service");
        c.SwaggerEndpoint("http://localhost:5005/swagger/v1/swagger.json", "Academic Service");
        c.SwaggerEndpoint("http://localhost:5003/swagger/v1/swagger.json", "Attendance Service");
        c.SwaggerEndpoint("http://localhost:5004/swagger/v1/swagger.json", "Exam Service");
        c.SwaggerEndpoint("http://localhost:5005/swagger/v1/swagger.json", "Grade Service");
        c.SwaggerEndpoint("http://localhost:5006/swagger/v1/swagger.json", "Notification Service");
        c.SwaggerEndpoint("http://localhost:5007/swagger/v1/swagger.json", "Message Service");
        c.SwaggerEndpoint("http://localhost:5008/swagger/v1/swagger.json", "Progress Service");
    });
}
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");

// 8. Correlation ID Middleware
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// 5. Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "API Gateway" }));

// 9. Map YARP
app.MapReverseProxy();

app.Run();

