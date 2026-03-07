namespace EmailService.Contracts
{
    public interface IEmailAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
