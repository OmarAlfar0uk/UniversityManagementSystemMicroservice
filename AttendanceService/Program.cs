using AttendanceService.Contracts;
using AttendanceService.Data;
using AttendanceService.Features.Attendance;
using AttendanceService.Middlewares;
using AttendanceService.Repositories;
using AttendanceService.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AttendanceService
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
            builder.Services.AddScoped<IAttendanceAuditLogger, AttendanceAuditLogger>();

            #region MediatR & Validation
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion
            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AttendanceDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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

            app.MapAttendanceEndpoints();

            app.Run();
        }
    }
}
