using Auth.Behaviors;
using Auth.Contarcts;
using Auth.Data.Seeding;
using Auth.Models;
using Auth.Repositories;
using Auth.Services;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Features.Auth;
using AuthService.Features.Auth.Admin;
using AuthService.Features.Auth.Parent;
using AuthService.Features.Auth.Student;
using AuthService.Features.Extensions;
using AuthService.Seeding;
using AuthService.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;


namespace Auth_Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Serilog
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"
               )

            .WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                shared: true)
            .CreateLogger();
            #endregion

            var builder = WebApplication.CreateBuilder(args);

            #region --- Services ---
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

                 #region Swagger Configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            #endregion

                 #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<UniversitySystemAuthContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion

                 #region Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // User
                options.User.RequireUniqueEmail = false;

                // Lockout
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.AllowedForNewUsers = true;
            })
              .AddEntityFrameworkStores<UniversitySystemAuthContext>()
              .AddDefaultTokenProviders();
            #endregion

                 #region JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
            #endregion

                 #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });
            #endregion

                 #region DI Registrations
            builder.Services.AddScoped<IImageHelper, ImageHelper>();
            builder.Host.UseSerilog();
            builder.Services.AddAuditLogging();
            builder.Services.AddScoped<IAuditLogger, AuditLogger>();
            builder.Services.AddCustomRateLimiting();
            builder.Services.AddScoped<ITokenService, JwtService>();
            builder.Services.AddScoped<IMailKitEmailService, MailKitEmailService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
            builder.Services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>)
            );

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });
            #endregion


            #endregion


            #region Caching
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();

            #endregion

            #endregion

            var app = builder.Build();

            #region --- Migration & Seeding ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    Console.WriteLine("📊 [Auth] Starting database migration...");

                    var context = services.GetRequiredService<UniversitySystemAuthContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                    // 1️⃣ Apply migrations
                    await context.Database.MigrateAsync();

                    // 2️⃣ Seed roles
                    await RoleSeeder.SeedRolesAsync(roleManager);

                    // 3️⃣ Seed SuperAdmin (depends on roles)
                    await SuperAdminSeeder.SeedSuperAdminAsync(userManager, roleManager);

                    Console.WriteLine("✅ [Auth] Database migration & seeding completed.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ [Auth] Error during migration/seeding: {ex.Message}");
                    Console.ResetColor();
                }
            }


            #endregion

            #region --- Pipeline ---
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseCors("AllowAll"); // CORS first

            // 🛡️ Global exception handler — must be first to catch everything
            app.UseMiddleware<Auth.Middlewares.GlobalExceptionMiddleware>();
            app.UseMiddleware<AuthService.Middlewares.CorrelationIdMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapGet("/", () => "Auth Service is running...");
            // Shared (Login, Logout, Activate, Refresh)
            app.MapAuthEndpoints();
            // Role-specific — each team owns their file
            app.MapAdminAuthEndpoints();
            app.MapParentAuthEndpoints();
            app.MapStudentAuthEndpoints();
            #endregion


            await app.RunAsync();
        }
    }
}
