using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public record GetCourseCatalogsQuery() : IRequest<IReadOnlyList<CourseCatalogResponse>>;
