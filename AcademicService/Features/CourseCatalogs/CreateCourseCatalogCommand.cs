using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public record CreateCourseCatalogCommand(
    string Name,
    string Code,
    string? Description,
    IFormFile? CoverImage) : IRequest<CourseCatalogResponse>;
