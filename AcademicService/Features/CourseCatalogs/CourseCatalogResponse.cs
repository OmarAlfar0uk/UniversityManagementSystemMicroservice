namespace AcademicService.Features.CourseCatalogs;

public record CourseCatalogResponse(
    Guid Id,
    string Name,
    string Code,
    string Description,
    string CoverImageUrl
);
