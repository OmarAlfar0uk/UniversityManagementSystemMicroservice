using AcademicService.Contracts;
using AcademicService.Consumers;
using AcademicService.Data;
using AcademicService.Features.Assignments;
using AcademicService.Features.CourseCatalogs;
using AcademicService.Features.Courses;
using AcademicService.Features.Departments;
using AcademicService.Features.LectureMaterials;
using AcademicService.Features.Lectures;
using AcademicService.Features.Schedule;
using AcademicService.Features.Internal;
using AcademicService.Middlewares;
using AcademicService.Repositories;
using AcademicService.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AcademicService.Features.Doctor;

namespace AcademicService
{
    public class Program
    {
        private const long MaxUploadSizeBytes = 500L * 1024 * 1024; // 500 MB

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = MaxUploadSizeBytes;
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = MaxUploadSizeBytes;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Academic API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid JWT token in the text input below. The 'Bearer' prefix will be added automatically.\r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });
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

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:4200",
                            "https://learnify-jqme.vercel.app",
                            "https://localhost:4200",
                            "https://learnify.tech",
                            "https://www.learnify.tech",
                            "https://academic.learnefy.tech",
                            "https://auth.learnefy.tech",
                            "https://reporting.learnefy.tech",
                            "https://progress.learnefy.tech"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            #endregion

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAcademicAuditLogger, AcademicAuditLogger>();

            // HTTP Clients
            builder.Services.AddHttpClient<IStudentDirectoryClient, AuthStudentDirectoryClient>();
            builder.Services.AddHttpClient("AuthService", client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["ServiceEndpoints:AuthServiceBaseUrl"] ?? "http://authservice:80");
            });
            builder.Services.AddScoped<IAuthServiceClient, AuthServiceClient>();

            // Helpers
            builder.Services.AddScoped<IImageHelper, ImageHelper>();
            builder.Services.AddScoped<IVideoHelper, VideoHelper>();
            builder.Services.AddScoped<IFileHelper, FileHelper>();

            #region MediatR & Validation
            builder.Services.AddMediatR(typeof(Program).Assembly);
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion


            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AcademicDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region JWT Configuration
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

            builder.Services.AddAuthorization();
            #endregion

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<UserUpdatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSettings = builder.Configuration.GetSection("RabbitMQ");
                    cfg.Host(rabbitSettings["Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(rabbitSettings["Username"] ?? "guest");
                        h.Password(rabbitSettings["Password"] ?? "guest");
                    });

                    cfg.ReceiveEndpoint("academic-user-updated-queue", e =>
                    {
                        e.ConfigureConsumer<UserUpdatedConsumer>(context);
                    });
                });
            });
            #endregion

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AcademicDbContext>();
                    await dbContext.Database.MigrateAsync();
                    await EnsureScheduleDepartmentSchemaAsync(dbContext);

                    Console.WriteLine("ðŸ“Š [AcademicService] Starting database seeding...");
                    await AcademicService.Seeding.DataSeeder.SeedAsync(services);
                    Console.WriteLine("âœ… [AcademicService] Database seeding completed.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"âŒ [AcademicService] Error during seeding: {ex.Message}");
                    Console.ResetColor();
                }
            }

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<SerilogEnricherMiddleware>();
            app.UseStaticFiles();

           // app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            
            app.MapCourseEndpoints();
            app.MapDepartmentEndpoints();
            app.MapCourseCatalogEndpoints();
            app.MapLectureEndpoints();
            app.MapLectureMaterialEndpoints();
            app.MapAssignmentEndpoints();
            app.MapScheduleEndpoints();
            app.MapInternalEndpoints();

            app.MapDoctorLectureEndpoints();
            await app.RunAsync();
        }

        private static async Task EnsureScheduleDepartmentSchemaAsync(AcademicDbContext dbContext)
        {
            const string sql = """
                IF COL_LENGTH('dbo.Schedules', 'DepartmentId') IS NULL
                BEGIN
                    ALTER TABLE dbo.Schedules
                    ADD DepartmentId uniqueidentifier NOT NULL
                    CONSTRAINT DF_Schedules_DepartmentId DEFAULT ('00000000-0000-0000-0000-000000000000');
                END;

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Schedules_DepartmentId_Type'
                      AND object_id = OBJECT_ID('dbo.Schedules')
                )
                BEGIN
                    CREATE INDEX IX_Schedules_DepartmentId_Type
                    ON dbo.Schedules (DepartmentId, [Type]);
                END;
                """;

            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
