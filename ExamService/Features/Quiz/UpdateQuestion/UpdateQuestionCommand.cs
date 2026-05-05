using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.UpdateQuestion;

public record UpdateChoiceRequest(
    Guid ChoiceId,
    string Text,
    bool IsCorrect
);

public record UpdateQuestionCommand(
    Guid QuestionId,
    string Text,
    int Points,
    string? CorrectAnswer,
    List<UpdateChoiceRequest> Options) : IRequest<QuestionResponse>;
