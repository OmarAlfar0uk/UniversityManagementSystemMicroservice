using ExamService.Contracts;
using System.Security.Claims;

namespace ExamService.Services
{
    public class ExamAuditLogger : IExamAuditLogger
    {
        private readonly ILogger<ExamAuditLogger> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamAuditLogger(ILogger<ExamAuditLogger> logger, IHttpContextAccessor httpContextAccessor)
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
