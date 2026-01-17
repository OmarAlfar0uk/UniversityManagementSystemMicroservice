using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using System.Security.Claims;

namespace AuthService.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogger(
            UniversitySystemAuthContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string? userId = null,
            string? targetId = null,
            string? description = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var actorId =
                userId ??
                httpContext?.User.FindFirstValue("id");

            var ip =
                httpContext?.Connection.RemoteIpAddress?.ToString();

            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                Action = action,
                UserId = actorId,
                TargetId = targetId,
                Description = description,
                IpAddress = ip
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
