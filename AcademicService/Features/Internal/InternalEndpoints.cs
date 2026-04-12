using AcademicService.Contracts;

namespace AcademicService.Features.Internal;

public static class InternalEndpoints
{
    private static bool IsInternalRequest(HttpContext ctx) =>
        ctx.Request.Headers["X-Internal-Request"] == "true";

    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/v1/academic/internal/courses/count
        app.MapGet("/api/v1/academic/internal/courses/count", async (
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var count = await uow.Courses.CountAsync(_ => true);
            return Results.Ok(count);
        });

        // GET /api/v1/academic/internal/courses
        app.MapGet("/api/v1/academic/internal/courses", async (
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var courses = await uow.Courses.GetAllAsync();
            var result = new List<object>();

            foreach (var course in courses)
            {
                var enrolledCount = await uow.CourseEnrollments.CountAsync(e => e.CourseId == course.Id);
                result.Add(new
                {
                    id             = course.Id,
                    name           = course.Name,
                    coverImageUrl  = course.CoverImageUrl,
                    doctorId       = course.DoctorId,
                    enrolledCount
                });
            }

            return Results.Ok(result);
        });

        // GET /api/v1/academic/internal/courses/{courseId}/lectures
        app.MapGet("/api/v1/academic/internal/courses/{courseId:guid}/lectures", async (
            Guid courseId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var lectures = await uow.Lectures.GetAllAsync(l => l.CourseId == courseId);
            var result = lectures.Select(l => new
            {
                id         = l.Id,
                title      = l.Title,
                orderIndex = l.OrderIndex
            });

            return Results.Ok(result);
        });

        // GET /api/v1/academic/internal/courses/{courseId}/enrollment-count
        app.MapGet("/api/v1/academic/internal/courses/{courseId:guid}/enrollment-count", async (
            Guid courseId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var count = await uow.CourseEnrollments.CountAsync(e => e.CourseId == courseId);
            return Results.Ok(count);
        });

        // GET /api/v1/academic/internal/students/{studentId}/courses
        app.MapGet("/api/v1/academic/internal/students/{studentId:guid}/courses", async (
            Guid studentId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var enrollments = await uow.CourseEnrollments.GetAllAsync(e => e.StudentId == studentId);
            var result = new List<object>();

            foreach (var enrollment in enrollments)
            {
                var course = await uow.Courses.GetByIdAsync(enrollment.CourseId);
                if (course is null) continue;

                var enrolledCount = await uow.CourseEnrollments.CountAsync(e => e.CourseId == course.Id);
                result.Add(new
                {
                    id            = course.Id,
                    name          = course.Name,
                    coverImageUrl = course.CoverImageUrl,
                    doctorId      = course.DoctorId,
                    enrolledCount
                });
            }

            return Results.Ok(result);
        });

        // GET /api/v1/academic/internal/doctors/{doctorId}/courses
        app.MapGet("/api/v1/academic/internal/doctors/{doctorId:guid}/courses", async (
            Guid doctorId,
            IUnitOfWork uow,
            HttpContext ctx) =>
        {
            if (!IsInternalRequest(ctx)) return Results.Forbid();

            var courses = await uow.Courses.GetAllAsync(c => c.DoctorId == doctorId);
            var result = new List<object>();

            foreach (var course in courses)
            {
                var enrolledCount = await uow.CourseEnrollments.CountAsync(e => e.CourseId == course.Id);
                result.Add(new
                {
                    id            = course.Id,
                    name          = course.Name,
                    coverImageUrl = course.CoverImageUrl,
                    doctorId      = course.DoctorId,
                    enrolledCount
                });
            }

            return Results.Ok(result);
        });
    }
}
