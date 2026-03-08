using AcademicService.Contracts;
using AcademicService.Data;
using AcademicService.Data.Models;

namespace AcademicService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AcademicDbContext _context;

    private IGenericRepository<Course>? _courses;
    private IGenericRepository<CourseEnrollment>? _courseEnrollments;
    private IGenericRepository<Lecture>? _lectures;
    private IGenericRepository<LecturePdf>? _lecturePdfs;
    private IGenericRepository<LectureVideo>? _lectureVideos;
    private IGenericRepository<Assignment>? _assignments;
    private IGenericRepository<AssignmentSubmission>? _assignmentSubmissions;
    private IGenericRepository<Schedule>? _schedules;

    public UnitOfWork(AcademicDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Course> Courses =>
        _courses ??= new GenericRepository<Course>(_context);

    public IGenericRepository<CourseEnrollment> CourseEnrollments =>
        _courseEnrollments ??= new GenericRepository<CourseEnrollment>(_context);

    public IGenericRepository<Lecture> Lectures =>
        _lectures ??= new GenericRepository<Lecture>(_context);

    public IGenericRepository<LecturePdf> LecturePdfs =>
        _lecturePdfs ??= new GenericRepository<LecturePdf>(_context);

    public IGenericRepository<LectureVideo> LectureVideos =>
        _lectureVideos ??= new GenericRepository<LectureVideo>(_context);

    public IGenericRepository<Assignment> Assignments =>
        _assignments ??= new GenericRepository<Assignment>(_context);

    public IGenericRepository<AssignmentSubmission> AssignmentSubmissions =>
        _assignmentSubmissions ??= new GenericRepository<AssignmentSubmission>(_context);

    public IGenericRepository<Schedule> Schedules =>
        _schedules ??= new GenericRepository<Schedule>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
