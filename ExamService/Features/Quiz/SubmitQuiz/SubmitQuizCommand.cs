using ExamService.Contracts;
using MediatR;

namespace ExamService.Features.Quiz.SubmitQuiz;

public record AnswerRequest(Guid QuestionId, Guid? SelectedOptionId, string? AnswerText);

public record SubmitQuizCommand(
    Guid LectureId,
    Guid StudentId,
    List<AnswerRequest> Answers
) : IRequest<QuizResultResponse>;
