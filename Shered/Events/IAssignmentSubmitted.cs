namespace Shered.Events
{
    public interface IAssignmentSubmitted
    {
        Guid StudentId { get; }
        Guid AssignmentId { get; }
        DateTime SubmissionDate { get; }
    }
}
