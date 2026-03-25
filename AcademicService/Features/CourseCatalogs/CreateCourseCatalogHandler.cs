using AcademicService.Contracts;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public class CreateCourseCatalogHandler : IRequestHandler<CreateCourseCatalogCommand, CourseCatalogResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public CreateCourseCatalogHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<CourseCatalogResponse> Handle(
        CreateCourseCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _unitOfWork.CourseCatalogs.AnyAsync(catalog => catalog.Code == normalizedCode))
            throw new InvalidOperationException($"Course catalog code '{normalizedCode}' already exists.");

        string? coverImageUrl = null;
        if (request.CoverImage is not null)
            coverImageUrl = await _imageHelper.SaveImageAsync(request.CoverImage, "CourseCatalogs");

        var catalog = new CourseCatalog
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = normalizedCode,
            Description = request.Description,
            CoverImageUrl = coverImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CourseCatalogs.AddAsync(catalog);
        await _unitOfWork.SaveChangesAsync();

        return new CourseCatalogResponse(
            catalog.Id,
            catalog.Name,
            catalog.Code,
            catalog.Description ?? string.Empty,
            _imageHelper.GetImageUrl(catalog.CoverImageUrl ?? string.Empty) ?? string.Empty);
    }
}
