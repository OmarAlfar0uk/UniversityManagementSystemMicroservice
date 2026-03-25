using FluentValidation;

namespace AcademicService.Features.CourseCatalogs;

public class CreateCourseOfferingValidator : AbstractValidator<CreateCourseOfferingCommand>
{
    public CreateCourseOfferingValidator()
    {
        RuleFor(x => x.CourseCatalogId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
    }
}
