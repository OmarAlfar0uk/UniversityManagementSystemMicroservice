namespace AttendanceService.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Items[HeaderName] = correlationId.ToString();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId.ToString();
                return Task.CompletedTask;
            });

            using (_logger.BeginScope("{CorrelationId}", correlationId.ToString()))
            {
                await _next(context);
            }
        }
    }
}
