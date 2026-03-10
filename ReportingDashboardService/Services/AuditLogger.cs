using ReportingDashboardService.Contracts;
using System.Security.Claims;

namespace ReportingDashboardService.Services
{
    public class ReportingAuditLogger : IReportingAuditLogger
    {
        private readonly ILogger<ReportingAuditLogger> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportingAuditLogger(ILogger<ReportingAuditLogger> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var actorId = userId ?? httpContext?.User.FindFirstValue("id");
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                "Audit | Action={Action} | UserId={UserId} | TargetId={TargetId} | Ip={IpAddress} | Description={Description}",
                action,
                actorId ?? "Anonymous",
                targetId,
                ipAddress ?? "Unknown",
                description);

            return Task.CompletedTask;
        }
    }
}
