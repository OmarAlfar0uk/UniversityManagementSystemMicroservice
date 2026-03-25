using AcademicService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademicService.Data;

public class AcademicDbContext : DbContext
{
    public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CourseCatalog> CourseCatalogs => Set<CourseCatalog>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
    public DbSet<Lecture> Lectures => Set<Lecture>();
    public DbSet<LecturePdf> LecturePdfs => Set<LecturePdf>();
    public DbSet<LectureVideo> LectureVideos => Set<LectureVideo>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<Schedule> Schedules => Set<Schedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicDbContext).Assembly);
    }
}
