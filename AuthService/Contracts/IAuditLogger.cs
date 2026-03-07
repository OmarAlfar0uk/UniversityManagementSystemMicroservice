namespace AuthService.Contracts
{
    public interface IAuthAuditLogger
    {
        Task LogAsync(
            string action,
            string? userId = null,
            string? targetId = null,
            string? description = null);
    }
}
