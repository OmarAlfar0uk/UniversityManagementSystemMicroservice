using FluentValidation;

namespace ExamService.Features.Quiz.GetQuizStatus;

public class GetQuizStatusValidator : AbstractValidator<GetQuizStatusQuery>
{
    public GetQuizStatusValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
