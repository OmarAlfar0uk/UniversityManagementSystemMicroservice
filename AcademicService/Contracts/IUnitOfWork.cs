using AcademicService.Data.Models;

namespace AcademicService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Course> Courses { get; }
    IGenericRepository<CourseEnrollment> CourseEnrollments { get; }
    IGenericRepository<Lecture> Lectures { get; }
    IGenericRepository<LecturePdf> LecturePdfs { get; }
    IGenericRepository<LectureVideo> LectureVideos { get; }
    IGenericRepository<Assignment> Assignments { get; }
    IGenericRepository<AssignmentSubmission> AssignmentSubmissions { get; }
    IGenericRepository<Schedule> Schedules { get; }
    Task<int> SaveChangesAsync();
}
