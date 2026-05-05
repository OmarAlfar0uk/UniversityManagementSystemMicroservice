using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.GetQuizQuestions;

public class GetQuizQuestionsHandler : IRequestHandler<GetQuizQuestionsQuery, QuizResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizQuestionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuizResponse> Handle(GetQuizQuestionsQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        var questions = await _unitOfWork.QuizQuestions.GetAllAsync(q => q.QuizId == quiz.Id);

        var questionResponses = new List<QuestionResponse>();
        foreach (var q in questions.OrderBy(q => q.OrderIndex))
        {
            var options = await _unitOfWork.QuizQuestionOptions.GetAllAsync(o => o.QuizQuestionId == q.Id);
            questionResponses.Add(new QuestionResponse(
                q.Id,
                q.Text,
                q.Type.ToString(),
                q.Points,
                options.Select(o => new OptionResponse(o.Id, o.Text, o.IsCorrect)).ToList()
            ));
        }

        return new QuizResponse(quiz.Id, quiz.TimeLimitInMinutes, questionResponses);
    }
}
