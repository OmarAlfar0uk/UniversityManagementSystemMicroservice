using System.Collections.Generic;
using System.Security.Claims;

namespace EmailService.Middlewares
{
    public class SerilogEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SerilogEnricherMiddleware> _logger;

        public SerilogEnricherMiddleware(RequestDelegate next, ILogger<SerilogEnricherMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User?.FindFirst("id")?.Value
                         ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? "Anonymous";

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["UserId"] = userId,
                ["IP"] = ip
            }))
            {
                await _next(context);
            }
        }
    }
}
