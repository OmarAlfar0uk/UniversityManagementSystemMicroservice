using ExamService.Contracts;
using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.AddQuestion;

public record OptionRequest(string Text, bool IsCorrect);

public record AddQuestionCommand(
    Guid LectureId,
    string Text,
    string Type,
    int Points,
    int OrderIndex,
    string? CorrectAnswer,
    List<OptionRequest> Options
) : IRequest<QuestionResponse>;
