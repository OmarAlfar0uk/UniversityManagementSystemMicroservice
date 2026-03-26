namespace Shered.Events
{
    public interface ILectureAdded
    {
        Guid LectureId     { get; }
        Guid CourseId      { get; }
        string CourseName  { get; }
        string LectureTitle { get; }
        IReadOnlyList<Guid> StudentIds { get; }
    }
}
