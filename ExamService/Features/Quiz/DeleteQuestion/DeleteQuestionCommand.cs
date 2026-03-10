using MediatR;

namespace ExamService.Features.Quiz.DeleteQuestion;

public record DeleteQuestionCommand(Guid QuestionId) : IRequest;
