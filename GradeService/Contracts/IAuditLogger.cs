namespace GradeService.Contracts
{
    public interface IGradeAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
