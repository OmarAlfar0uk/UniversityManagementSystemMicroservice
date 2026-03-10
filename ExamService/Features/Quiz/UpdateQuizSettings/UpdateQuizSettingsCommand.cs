using MediatR;

namespace ExamService.Features.Quiz.UpdateQuizSettings;

public record UpdateQuizSettingsCommand(Guid QuizId, int TimeLimitInMinutes, int MaxAttempts) : IRequest;
