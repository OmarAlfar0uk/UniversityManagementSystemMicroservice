using FluentValidation;

namespace ProgressService.Features.Progress.GetLectureProgress;

public class GetLectureProgressValidator : AbstractValidator<GetLectureProgressQuery>
{
    public GetLectureProgressValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
