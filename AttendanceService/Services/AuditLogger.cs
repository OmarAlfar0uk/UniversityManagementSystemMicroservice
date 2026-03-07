using AttendanceService.Contracts;
using System.Security.Claims;

namespace AttendanceService.Services
{
    public class AttendanceAuditLogger : IAttendanceAuditLogger
    {
        private readonly ILogger<AttendanceAuditLogger> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AttendanceAuditLogger(ILogger<AttendanceAuditLogger> logger, IHttpContextAccessor httpContextAccessor)
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

