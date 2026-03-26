using FluentValidation;

namespace AuthService.Features.Auth.Student.GetProfile;

public class GetProfileValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
