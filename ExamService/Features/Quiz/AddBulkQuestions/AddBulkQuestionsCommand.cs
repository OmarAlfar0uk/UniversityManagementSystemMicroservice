using ExamService.Features.Quiz.GetQuizQuestions;
using ExamService.Features.Quiz.AddQuestion;
using MediatR;

namespace ExamService.Features.Quiz.AddBulkQuestions;

public record BulkQuestionRequest(
    string Text,
    string Type,
    int Points,
    int OrderIndex,
    string? CorrectAnswer,
    List<OptionRequest> Options);

public record AddBulkQuestionsCommand(
    Guid QuizId,
    List<BulkQuestionRequest> Questions
) : IRequest<List<QuestionResponse>>;
