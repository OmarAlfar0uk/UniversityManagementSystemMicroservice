using FluentValidation;

namespace AcademicService.Features.Lectures.AddLecture;

public class AddLectureValidator : AbstractValidator<AddLectureCommand>
{
    public AddLectureValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}
