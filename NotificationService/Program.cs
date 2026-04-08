using FluentValidation;
using NotificationService.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using NotificationService.Contracts;
using NotificationService.Data;
using NotificationService.Features.Notifications;
using NotificationService.Middlewares;
using NotificationService.Repositories;
using NotificationService.Services;
using MassTransit;
using NotificationService.Consumers;

namespace NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification API", Version = "v1" });
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
                options.AddPolicy("AllowAll", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });
            #endregion

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<INotificationAuditLogger, NotificationAuditLogger>();

            #region MediatR & Validation
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion
            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region JWT Configuration
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

                    // CRITICAL: SignalR sends token via query string for WebSocket connections
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs/notifications"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSignalR();

            builder.Services.AddAuthorization();
            #endregion

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<AuthCreatedConsumer>();
                x.AddConsumer<LectureAddedConsumer>();
                x.AddConsumer<AssignmentAddedConsumer>();
                x.AddConsumer<AttendanceRegisteredConsumer>();
                x.AddConsumer<QuizCompletedConsumer>();
                x.AddConsumer<GradeAddedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSettings = builder.Configuration.GetSection("RabbitMQ");
                    cfg.Host(rabbitSettings["Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(rabbitSettings["Username"] ?? "guest");
                        h.Password(rabbitSettings["Password"] ?? "guest");
                    });

                    cfg.ReceiveEndpoint("auth-created-queue", e =>
                    {
                        e.ConfigureConsumer<AuthCreatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("lecture-added-queue", e =>
                    {
                        e.ConfigureConsumer<LectureAddedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("assignment-added-queue", e =>
                    {
                        e.ConfigureConsumer<AssignmentAddedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("attendance-registered-queue", e =>
                    {
                        e.ConfigureConsumer<AttendanceRegisteredConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("quiz-completed-queue", e =>
                    {
                        e.ConfigureConsumer<QuizCompletedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("grade-added-queue", e =>
                    {
                        e.ConfigureConsumer<GradeAddedConsumer>(context);
                    });
                });
            });
            #endregion

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //}
                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseCors("AllowAll");
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<SerilogEnricherMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapNotificationEndpoints();

            // ✅ SignalR hub endpoint
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
