using ExamService.Features.Quiz.GetQuizQuestions;
using MediatR;

namespace ExamService.Features.Quiz.UpdateQuestion;

public record UpdateQuestionCommand(Guid QuestionId, string Text, int Points, string? CorrectAnswer) : IRequest<QuestionResponse>;
