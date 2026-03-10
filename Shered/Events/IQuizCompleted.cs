namespace Shered.Events
{
    public interface IQuizCompleted
    {
        Guid StudentId { get; }
        Guid QuizId { get; }
        decimal Score { get; }
        bool IsPassed { get; }
        DateTime CompletedAt { get; }
    }
}
