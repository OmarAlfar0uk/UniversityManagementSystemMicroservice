using GradeService.Contracts;
using GradeService.Data;
using GradeService.Features.Grades;
using GradeService.Middlewares;
using GradeService.Repositories;
using GradeService.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GradeService.Consumers;

namespace GradeService
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
            builder.Services.AddScoped<IGradeAuditLogger, GradeAuditLogger>();

            #region MediatR & Validation
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion
            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<GradeDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<AssignmentSubmittedConsumer>();
                x.AddConsumer<QuizCompletedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("assignment-submitted-queue", e =>
                    {
                        e.ConfigureConsumer<AssignmentSubmittedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("quiz-completed-queue", e =>
                    {
                        e.ConfigureConsumer<QuizCompletedConsumer>(context);
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

            app.MapGradeEndpoints();

            app.Run();
        }
    }
}
