using FluentValidation;

namespace ExamService.Features.Quiz.GetQuizResult;

public class GetQuizResultValidator : AbstractValidator<GetQuizResultQuery>
{
    public GetQuizResultValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
