using FluentValidation;

namespace AcademicService.Features.Departments.GetDepartmentDetails;

public class GetDepartmentDetailsValidator : AbstractValidator<GetDepartmentDetailsQuery>
{
    public GetDepartmentDetailsValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty();
    }
}
