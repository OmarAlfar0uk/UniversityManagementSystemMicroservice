using FluentValidation;

namespace ExamService.Features.Quiz.GetQuizQuestions;

public class GetQuizQuestionsValidator : AbstractValidator<GetQuizQuestionsQuery>
{
    public GetQuizQuestionsValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
