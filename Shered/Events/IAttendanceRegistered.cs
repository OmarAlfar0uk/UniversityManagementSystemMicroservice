namespace Shered.Events
{
    public interface IAttendanceRegistered
    {
        Guid StudentId   { get; }
        Guid CourseId    { get; }
        Guid LectureId   { get; }
        DateTime Date    { get; }
    }
}
