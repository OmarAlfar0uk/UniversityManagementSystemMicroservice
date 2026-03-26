using Serilog.Context;

namespace ReportingDashboardService.Middlewares
{
    public class SerilogEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("UserId",
                context.User?.FindFirst("sub")?.Value ?? "anonymous"))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            {
                await _next(context);
            }
        }
    }
}
