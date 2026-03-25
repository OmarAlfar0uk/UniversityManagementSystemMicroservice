namespace AcademicService.Data.Models;

public class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CourseCatalogId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Lecture> Lectures { get; set; } = [];
    public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
    public Department? Department { get; set; }
    public CourseCatalog? CourseCatalog { get; set; }
}
