using System.Security.Claims;
using ExamService.Features.Quiz.AddQuestion;
using ExamService.Features.Quiz.CreateQuiz;
using ExamService.Features.Quiz.DeleteQuestion;
using ExamService.Features.Quiz.GetQuizQuestions;
using ExamService.Features.Quiz.GetQuizResult;
using ExamService.Features.Quiz.GetQuizReview;
using ExamService.Features.Quiz.GetQuizStatus;
using ExamService.Features.Quiz.GradeEssay;
using ExamService.Features.Quiz.SubmitQuiz;
using ExamService.Features.Quiz.UpdateQuestion;
using ExamService.Features.Quiz.UpdateQuizSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.Features.Quiz;

public static class Endpoints
{
    public static void MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        // Student endpoints
        var student = app.MapGroup("/api/v1/exam/lectures")
                         .RequireAuthorization()
                         .WithTags("Exam – Quiz (Student)");

        student.MapGet("/{lectureId:guid}/quiz", async (Guid lectureId, ISender sender) =>
        {
            var result = await sender.Send(new GetQuizQuestionsQuery(lectureId));
            return Results.Ok(result);
        }).WithSummary("Get quiz questions (Student)");

        student.MapGet("/{lectureId:guid}/quiz/status", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetQuizStatusQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get quiz attempt status (Student)");

        student.MapPost("/{lectureId:guid}/quiz/submit", async (
            Guid lectureId,
            [FromBody] SubmitBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new SubmitQuizCommand(lectureId, studentId, body.Answers));
            return Results.Ok(result);
        }).WithSummary("Submit quiz answers (Student)");

        student.MapGet("/{lectureId:guid}/quiz/result", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetQuizResultQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get quiz result (Student)");

        student.MapGet("/{lectureId:guid}/quiz/review", async (
            Guid lectureId, ISender sender, ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetQuizReviewQuery(lectureId, studentId));
            return Results.Ok(result);
        }).WithSummary("Get quiz review with correct answers (Student)");

        // Doctor / Admin endpoints
        var doctor = app.MapGroup("/api/v1/exam/admin")
                        .RequireAuthorization()
                        .WithTags("Exam – Quiz (Admin/Doctor)");

        doctor.MapPost("/lectures/{lectureId:guid}/quiz", async (
            Guid lectureId,
            [FromBody] CreateQuizBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new CreateQuizCommand(lectureId, body.CourseId, body.TimeLimitInMinutes, body.MaxAttempts));
            return Results.Created($"/api/v1/exam/lectures/{lectureId}/quiz", result);
        }).WithSummary("Create quiz for a lecture (Admin/Doctor)");

        doctor.MapPost("/lectures/{lectureId:guid}/quiz/questions", async (
            Guid lectureId,
            [FromBody] AddQuestionBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new AddQuestionCommand(
                lectureId, body.Text, body.Type, body.Points, body.OrderIndex, body.CorrectAnswer, body.Options));
            return Results.Created($"/api/v1/exam/admin/questions/{result.Id}", result);
        }).WithSummary("Add question to quiz (Admin/Doctor)");

        doctor.MapPut("/questions/{questionId:guid}", async (
            Guid questionId,
            [FromBody] UpdateQuestionBody body,
            ISender sender) =>
        {
            var result = await sender.Send(new UpdateQuestionCommand(questionId, body.Text, body.Points, body.CorrectAnswer));
            return Results.Ok(result);
        }).WithSummary("Update question (Admin/Doctor)");

        doctor.MapDelete("/questions/{questionId:guid}", async (Guid questionId, ISender sender) =>
        {
            await sender.Send(new DeleteQuestionCommand(questionId));
            return Results.NoContent();
        }).WithSummary("Delete question (Admin/Doctor)");

        doctor.MapPut("/quizzes/{quizId:guid}/settings", async (
            Guid quizId,
            [FromBody] QuizSettingsBody body,
            ISender sender) =>
        {
            await sender.Send(new UpdateQuizSettingsCommand(quizId, body.TimeLimitInMinutes, body.MaxAttempts));
            return Results.Ok(new { message = "Quiz settings updated." });
        }).WithSummary("Update quiz settings (Admin/Doctor)");

        doctor.MapPut("/quizzes/{quizId:guid}/attempts/{attemptId:guid}/questions/{questionId:guid}/grade", async (
            Guid quizId, Guid attemptId, Guid questionId,
            [FromBody] GradeEssayBody body,
            ISender sender) =>
        {
            await sender.Send(new GradeEssayCommand(quizId, attemptId, questionId, body.EarnedPoints));
            return Results.Ok(new { message = "Essay graded." });
        }).WithSummary("Grade essay answer (Doctor)");
    }
}

public record SubmitBody(List<AnswerRequest> Answers);
public record CreateQuizBody(Guid CourseId, int TimeLimitInMinutes, int MaxAttempts);
public record AddQuestionBody(string Text, string Type, int Points, int OrderIndex, string? CorrectAnswer, List<OptionRequest> Options);
public record UpdateQuestionBody(string Text, int Points, string? CorrectAnswer);
public record QuizSettingsBody(int TimeLimitInMinutes, int MaxAttempts);
public record GradeEssayBody(decimal EarnedPoints);
