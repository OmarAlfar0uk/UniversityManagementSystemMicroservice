using AcademicService.Contracts;
using AcademicService.Data;
using AcademicService.Features.Assignments;
using AcademicService.Features.Courses;
using AcademicService.Features.LectureMaterials;
using AcademicService.Features.Lectures;
using AcademicService.Features.Schedule;
using AcademicService.Middlewares;
using AcademicService.Repositories;
using AcademicService.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AcademicService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            builder.Services.AddScoped<IAcademicAuditLogger, AcademicAuditLogger>();
            
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
            #endregion

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSettings = builder.Configuration.GetSection("RabbitMQ");
                    cfg.Host(rabbitSettings["Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(rabbitSettings["Username"] ?? "guest");
                        h.Password(rabbitSettings["Password"] ?? "guest");
                    });
                });
            });
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<SerilogEnricherMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapCourseEndpoints();
            app.MapLectureEndpoints();
            app.MapLectureMaterialEndpoints();
            app.MapAssignmentEndpoints();
            app.MapScheduleEndpoints();

            app.Run();
        }
    }
}
