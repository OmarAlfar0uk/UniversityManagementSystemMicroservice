namespace ReportingDashboardService.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeader = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(CorrelationIdHeader))
            {
                context.Request.Headers[CorrelationIdHeader] = Guid.NewGuid().ToString();
            }

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                {
                    context.Response.Headers[CorrelationIdHeader] =
                        context.Request.Headers[CorrelationIdHeader];
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
