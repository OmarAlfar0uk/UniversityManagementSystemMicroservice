using AuthService.Contracts;
using AuthService.Services;

namespace AuthService.Features.Extensions
{
    public static class AuditExtensions
    {
        public static IServiceCollection AddAuditLogging(
            this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthAuditLogger, AuthAuditLogger>();

            return services;
        }
    }
}
