using ExamService.Contracts;
using ExamService.Data.Enums;
using ExamService.Data.Models;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.AddBulkQuestions;

public class AddBulkQuestionsHandler : IRequestHandler<AddBulkQuestionsCommand, List<QuestionResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddBulkQuestionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<QuestionResponse>> Handle(AddBulkQuestionsCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for id {request.QuizId}.");

        var responses = new List<QuestionResponse>(request.Questions.Count);

        foreach (var item in request.Questions)
        {
            if (!Enum.TryParse<QuestionType>(item.Type, out var questionType))
                throw new ArgumentException($"Invalid question type: {item.Type}");

            var question = new QuizQuestion
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Text = item.Text,
                Type = questionType,
                Points = item.Points,
                OrderIndex = item.OrderIndex,
                CorrectAnswer = item.CorrectAnswer,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.QuizQuestions.AddAsync(question);

            var optionResponses = new List<OptionResponse>();
            foreach (var opt in item.Options)
            {
                var option = new QuizQuestionOption
                {
                    Id = Guid.NewGuid(),
                    QuizQuestionId = question.Id,
                    Text = opt.Text,
                    IsCorrect = opt.IsCorrect,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.QuizQuestionOptions.AddAsync(option);
                optionResponses.Add(new OptionResponse(option.Id, option.Text, option.IsCorrect));
            }

            responses.Add(new QuestionResponse(
                question.Id,
                question.Text,
                question.Type.ToString(),
                question.Points,
                optionResponses));
        }

        await _unitOfWork.SaveChangesAsync();
        return responses;
    }
}
