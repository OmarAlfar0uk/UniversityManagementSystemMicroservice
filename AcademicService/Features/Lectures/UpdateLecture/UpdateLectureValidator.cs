using FluentValidation;

namespace AcademicService.Features.Lectures.UpdateLecture;

public class UpdateLectureValidator : AbstractValidator<UpdateLectureCommand>
{
    public UpdateLectureValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}
