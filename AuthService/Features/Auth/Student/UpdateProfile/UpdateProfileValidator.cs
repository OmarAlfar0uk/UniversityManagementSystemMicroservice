using FluentValidation;

namespace AuthService.Features.Auth.Student.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public UpdateProfileValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");

        When(x => x.FullName is not null, () =>
        {
            RuleFor(x => x.FullName)
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        });

        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Phone number is not valid.");
        });

        When(x => x.ProfileImage is not null, () =>
        {
            RuleFor(x => x.ProfileImage!)
                .Must(f => f.Length <= 10L * 1024 * 1024)
                    .WithMessage("Profile image must not exceed 10 MB.")
                .Must(f => AllowedExtensions.Contains(
                    Path.GetExtension(f.FileName).ToLowerInvariant()))
                    .WithMessage("Only image files are allowed (jpg, jpeg, png, gif, webp).");
        });
    }
}
