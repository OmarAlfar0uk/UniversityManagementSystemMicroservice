namespace AttendanceService.Contracts
{
    public interface IAttendanceAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
