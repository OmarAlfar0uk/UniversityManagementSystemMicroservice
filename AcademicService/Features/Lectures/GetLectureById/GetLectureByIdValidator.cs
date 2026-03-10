using FluentValidation;

namespace AcademicService.Features.Lectures.GetLectureById;

public class GetLectureByIdValidator : AbstractValidator<GetLectureByIdQuery>
{
    public GetLectureByIdValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
