using FluentValidation;

namespace AcademicService.Features.CourseCatalogs;

public class CreateCourseCatalogValidator : AbstractValidator<CreateCourseCatalogCommand>
{
    public CreateCourseCatalogValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}
