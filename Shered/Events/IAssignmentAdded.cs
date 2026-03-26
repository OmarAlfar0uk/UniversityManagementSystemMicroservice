namespace Shered.Events
{
    public interface IAssignmentAdded
    {
        Guid AssignmentId  { get; }
        Guid LectureId     { get; }
        Guid CourseId      { get; }
        string CourseName  { get; }
        string Title       { get; }
        DateTime? DueDate  { get; }
        IReadOnlyList<Guid> StudentIds { get; }
    }
}
