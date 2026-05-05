using ExamService.Contracts;
using ExamService.Data.Models;
using ExamService.Data.Enums;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.AddQuestion;

public class AddQuestionHandler : IRequestHandler<AddQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<QuestionResponse> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Quizzes.FindAsync(q => q.LectureId == request.LectureId);
        if (quiz is null)
            throw new KeyNotFoundException($"No quiz found for lecture {request.LectureId}.");

        if (!Enum.TryParse<QuestionType>(request.Type, out var questionType))
            throw new ArgumentException($"Invalid question type: {request.Type}");

        var question = new QuizQuestion
        {
            Id = Guid.NewGuid(),
            QuizId = quiz.Id,
            Text = request.Text,
            Type = questionType,
            Points = request.Points,
            OrderIndex = request.OrderIndex,
            CorrectAnswer = request.CorrectAnswer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.QuizQuestions.AddAsync(question);

        var optionResponses = new List<OptionResponse>();
        foreach (var opt in request.Options)
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

        await _unitOfWork.SaveChangesAsync();

        return new QuestionResponse(question.Id, question.Text, question.Type.ToString(), question.Points, optionResponses);
    }
}
