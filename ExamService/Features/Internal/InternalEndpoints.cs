using ExamService.Contracts;

namespace ExamService.Features.Internal;

public static class InternalEndpoints
{
    private static bool IsInternalRequest(HttpContext ctx) =>
        ctx.Request.Headers["X-Internal-Request"] == "true";

    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/v1/exam/internal/students/{studentId}/quiz-results
        app.MapGet("/api/v1/exam/internal/students/{studentId:guid}/quiz-results", async (
            Guid studentId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var attempts = await uow.QuizAttempts.GetAllAsync(a => a.StudentId == studentId);
            var result   = new List<object>();

            foreach (var attempt in attempts)
            {
                var quiz = await uow.Quizzes.GetByIdAsync(attempt.QuizId);
                if (quiz is null) continue;

                result.Add(new
                {
                    lectureId = quiz.LectureId,
                    score     = attempt.Score,
                    isPassed  = attempt.IsPassed
                });
            }

            return Results.Ok(result);
        });

        // GET /api/v1/exam/internal/doctors/{doctorId}/pending-essays/count
        app.MapGet("/api/v1/exam/internal/doctors/{doctorId:guid}/pending-essays/count", async (
            Guid doctorId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            // Count all ungraded (IsCorrect == null) quiz answers
            var count = await uow.QuizAnswers.CountAsync(a => a.IsCorrect == null);
            return Results.Ok(count);
        });

        // GET /api/v1/exam/internal/courses/{courseId}/quiz-stats
        app.MapGet("/api/v1/exam/internal/courses/{courseId:guid}/quiz-stats", async (
            Guid courseId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var quizzes = await uow.Quizzes.GetAllAsync(q => q.CourseId == courseId);
            var result  = new List<object>();

            foreach (var quiz in quizzes)
            {
                var attempts      = await uow.QuizAttempts.GetAllAsync(a => a.QuizId == quiz.Id);
                var attemptList   = attempts.ToList();
                var totalAttempts = attemptList.Count;
                var passedCount   = attemptList.Count(a => a.IsPassed == true);
                var averageScore  = totalAttempts > 0
                    ? Math.Round((double)attemptList
                        .Where(a => a.Score.HasValue)
                        .Select(a => (double)a.Score!.Value)
                        .DefaultIfEmpty(0)
                        .Average(), 2)
                    : 0.0;
                var passRate = totalAttempts > 0
                    ? Math.Round((double)passedCount / totalAttempts * 100, 2)
                    : 0.0;

                result.Add(new
                {
                    quizId        = quiz.Id,
                    averageScore,
                    totalAttempts,
                    passedCount,
                    passRate
                });
            }

            return Results.Ok(result);
        });
    }
}
