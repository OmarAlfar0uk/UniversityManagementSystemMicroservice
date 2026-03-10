using FluentValidation;

namespace ExamService.Features.Quiz.SubmitQuiz;

public class SubmitQuizValidator : AbstractValidator<SubmitQuizCommand>
{
    public SubmitQuizValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Answers).NotEmpty();
    }
}
