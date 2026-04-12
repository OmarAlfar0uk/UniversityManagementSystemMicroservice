using AcademicService.Contracts;

namespace AcademicService.Features.Internal;

public static class InternalEndpoints
{
    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/academic/internal")
                       .RequireAuthorization()
                       .WithTags("Internal \u2013 Academic");

        // GET /api/v1/academic/internal/courses/count
        group.MapGet("/courses/count", async (IUnitOfWork uow) =>
        {
            var count = await uow.Courses.CountAsync(_ => true);
            return Results.Ok(count);
        });

        // GET /api/v1/academic/internal/courses
        group.MapGet("/courses", async (IUnitOfWork uow) =>
        {
            var courses = await uow.Courses.GetAllAsync();
            var result  = new List<object>();

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

        // GET /api/v1/academic/internal/courses/{courseId}/lectures
        group.MapGet("/courses/{courseId:guid}/lectures", async (
            Guid courseId,
            IUnitOfWork uow) =>
        {
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
        group.MapGet("/courses/{courseId:guid}/enrollment-count", async (
            Guid courseId,
            IUnitOfWork uow) =>
        {
            var count = await uow.CourseEnrollments.CountAsync(e => e.CourseId == courseId);
            return Results.Ok(count);
        });

        // GET /api/v1/academic/internal/students/{studentId}/courses
        group.MapGet("/students/{studentId:guid}/courses", async (
            Guid studentId,
            IUnitOfWork uow) =>
        {
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
        group.MapGet("/doctors/{doctorId:guid}/courses", async (
            Guid doctorId,
            IUnitOfWork uow) =>
        {
            var courses = await uow.Courses.GetAllAsync(c => c.DoctorId == doctorId);
            var result  = new List<object>();

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
