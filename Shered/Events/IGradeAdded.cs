namespace Shered.Events
{
    public interface IGradeAdded
    {
        Guid StudentId   { get; }
        Guid CourseId    { get; }
        string GradeType { get; }   // "Midterm" or "Final"
        decimal Score    { get; }
    }
}
