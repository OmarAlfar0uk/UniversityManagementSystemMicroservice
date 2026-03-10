using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddSwaggerGen();

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

            #region massTransit with RabbitMQ
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<AuthCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("auth-created-queue", e =>
                    {
                        e.ConfigureConsumer<AuthCreatedConsumer>(context);
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

            app.MapNotificationEndpoints();

            app.Run();
        }
    }
}
