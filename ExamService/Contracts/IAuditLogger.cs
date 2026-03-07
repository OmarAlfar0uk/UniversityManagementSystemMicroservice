namespace ExamService.Contracts
{
    public interface IExamAuditLogger
    {
        Task LogAsync(string action, string? userId = null, string? targetId = null, string? description = null);
    }
}
