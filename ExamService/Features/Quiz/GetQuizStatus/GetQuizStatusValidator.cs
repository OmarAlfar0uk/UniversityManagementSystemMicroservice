using FluentValidation;

namespace ExamService.Features.Quiz.GetQuizStatus;

public class GetQuizStatusValidator : AbstractValidator<GetQuizStatusQuery>
{
    public GetQuizStatusValidator()
    {
        RuleFor(x => x.QuizOrLectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
