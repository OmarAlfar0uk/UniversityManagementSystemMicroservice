using FluentValidation;
using MediatR;
using MessageService.Contracts;
using MessageService.Data;
using MessageService.Features.Messages;
using MessageService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessageService
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

            #region MediatR & Validation
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            #endregion

            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MessageDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.MapMessageEndpoints();

            app.Run();
        }
    }
}
