using GradeService.Contracts;

namespace GradeService.Features.Internal;

public static class InternalEndpoints
{
    private static bool IsInternalRequest(HttpContext ctx) =>
        ctx.Request.Headers["X-Internal-Request"] == "true";

    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/v1/grade/internal/students/{studentId}
        app.MapGet("/api/v1/grade/internal/students/{studentId:guid}", async (
            Guid studentId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var grades = await uow.StudentGrades.GetAllAsync(g => g.StudentId == studentId);
            var result = grades.Select(g => new
            {
                courseId      = g.CourseId,
                midtermScore  = g.MidtermScore,
                finalScore    = g.FinalScore,
                totalScore    = g.TotalScore
            });

            return Results.Ok(result);
        });

        // GET /api/v1/grade/internal/students/{studentId}/gpa
        app.MapGet("/api/v1/grade/internal/students/{studentId:guid}/gpa", async (
            Guid studentId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var grades = await uow.StudentGrades.GetAllAsync(g => g.StudentId == studentId);
            var gradeList = grades.ToList();

            if (gradeList.Count == 0) return Results.Ok((double?)null);

            var gpa = gradeList
                .Select(g => MapToGpaScale((double)g.TotalScore))
                .Average();

            return Results.Ok((double?)Math.Round(gpa, 2));
        });

        // GET /api/v1/grade/internal/courses/{courseId}/grades
        app.MapGet("/api/v1/grade/internal/courses/{courseId:guid}/grades", async (
            Guid courseId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var grades = await uow.StudentGrades.GetAllAsync(g => g.CourseId == courseId);
            var result = grades.Select(g => new
            {
                studentId    = g.StudentId,
                midtermScore = g.MidtermScore,
                finalScore   = g.FinalScore,
                totalScore   = g.TotalScore
            });

            return Results.Ok(result);
        });

        // GET /api/v1/grade/internal/courses/{courseId}/average
        app.MapGet("/api/v1/grade/internal/courses/{courseId:guid}/average", async (
            Guid courseId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var grades = await uow.StudentGrades.GetAllAsync(g => g.CourseId == courseId);
            var gradeList = grades.ToList();

            if (gradeList.Count == 0) return Results.Ok(0.0);

            var average = gradeList.Average(g => (double)g.TotalScore);
            return Results.Ok(Math.Round(average, 2));
        });
    }

    private static double MapToGpaScale(double totalScore) => totalScore switch
    {
        >= 90 => 4.0,
        >= 80 => 3.0,
        >= 70 => 2.0,
        >= 60 => 1.0,
        _     => 0.0
    };
}
