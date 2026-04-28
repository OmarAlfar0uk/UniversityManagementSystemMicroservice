using FluentValidation;

namespace AcademicService.Features.Schedule.UploadSchedule;

public class UploadScheduleValidator : AbstractValidator<UploadScheduleCommand>
{
    private static readonly string[] AllowedExtensions =
        { ".jpg", ".jpeg", ".png", ".webp" };

    private static readonly string[] AllowedTypes =
        { "ClassSchedule", "MidtermExamSchedule" };

    public UploadScheduleValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("DepartmentId is required.");

        RuleFor(x => x.Type)
            .Must(t => AllowedTypes.Contains(t))
            .WithMessage("Type must be 'ClassSchedule' or 'MidtermExamSchedule'.");

        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image file is required.")
            .Must(f => f.Length <= 5 * 1024 * 1024)
                .WithMessage("Image must not exceed 5 MB.")
            .Must(f => AllowedExtensions.Contains(
                Path.GetExtension(f.FileName).ToLowerInvariant()))
                .WithMessage("Only image files are allowed (jpg, jpeg, png, webp).");
    }
}
