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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAcademicAuditLogger, AcademicAuditLogger>();
            
            // Helpers
            builder.Services.AddScoped<IImageHelper, ImageHelper>();
            builder.Services.AddScoped<IVideoHelper, VideoHelper>();
            builder.Services.AddScoped<IFileHelper, FileHelper>();

            #region MediatR & Validation
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion


            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AcademicDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

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

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<SerilogEnricherMiddleware>();

            app.UseHttpsRedirection();
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
