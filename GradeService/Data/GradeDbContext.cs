using GradeService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeService.Data;

public class GradeDbContext : DbContext
{
    public GradeDbContext(DbContextOptions<GradeDbContext> options) : base(options) { }

    public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GradeDbContext).Assembly);
    }
}
