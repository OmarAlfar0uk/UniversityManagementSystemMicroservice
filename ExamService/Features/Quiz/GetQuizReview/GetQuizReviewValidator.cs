using FluentValidation;

namespace ExamService.Features.Quiz.GetQuizReview;

public class GetQuizReviewValidator : AbstractValidator<GetQuizReviewQuery>
{
    public GetQuizReviewValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
