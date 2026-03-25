using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public class GetCourseCatalogsHandler : IRequestHandler<GetCourseCatalogsQuery, IReadOnlyList<CourseCatalogResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetCourseCatalogsHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<IReadOnlyList<CourseCatalogResponse>> Handle(
        GetCourseCatalogsQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await _unitOfWork.CourseCatalogs.GetAllAsync();

        return catalogs
            .OrderBy(catalog => catalog.Name)
            .Select(catalog => new CourseCatalogResponse(
                catalog.Id,
                catalog.Name,
                catalog.Code,
                catalog.Description ?? string.Empty,
                _imageHelper.GetImageUrl(catalog.CoverImageUrl ?? string.Empty) ?? string.Empty))
            .ToList();
    }
}
