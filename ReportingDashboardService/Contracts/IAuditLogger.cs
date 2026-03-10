namespace ReportingDashboardService.Contracts
{
    public interface IReportingAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
