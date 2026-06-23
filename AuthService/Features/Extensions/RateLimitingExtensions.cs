using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace AuthService.Features.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(
            this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            ip,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0
                            });
                    });

                options.AddPolicy("login", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        ip,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("activate", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        ip,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });


                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.Headers["Retry-After"] = "60";

                    var body = new
                    {
                        message = "Too many requests. Please try again after 60 seconds.",
                        retryAfter = 60
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(body, token);
                };

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
