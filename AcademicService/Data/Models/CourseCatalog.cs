namespace AcademicService.Data.Models;

public class CourseCatalog
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Course> Offerings { get; set; } = [];
}
