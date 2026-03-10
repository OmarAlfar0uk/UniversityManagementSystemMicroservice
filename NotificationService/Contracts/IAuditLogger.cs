namespace NotificationService.Contracts
{
    public interface INotificationAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
