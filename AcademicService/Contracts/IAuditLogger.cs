namespace AcademicService.Contracts
{
    public interface IAcademicAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
