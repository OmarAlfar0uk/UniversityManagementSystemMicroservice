using FluentValidation;

namespace AcademicService.Features.Courses.UpdateCourse;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
        RuleFor(x => x.DoctorId).NotEmpty();

        When(x => x.CoverImage is not null, () =>
        {
            RuleFor(x => x.CoverImage!)
                .Must(f => f.Length <= 2 * 1024 * 1024)
                    .WithMessage("Cover image must not exceed 2 MB.")
                .Must(f => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                    .Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
                    .WithMessage("Only image files are allowed (jpg, jpeg, png, gif, webp).");
        });
    }
}
