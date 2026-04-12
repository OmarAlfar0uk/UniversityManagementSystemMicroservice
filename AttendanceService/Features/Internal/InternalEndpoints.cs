using AttendanceService.Contracts;

namespace AttendanceService.Features.Internal;

public static class InternalEndpoints
{
    public static void MapInternalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/attendance/internal")
                       .RequireAuthorization()
                       .WithTags("Internal \u2013 Attendance");

        // GET /api/v1/attendance/internal/students/{studentId}
        group.MapGet("/students/{studentId:guid}", async (
            Guid studentId,
            IUnitOfWork uow) =>
        {
            var records = await uow.AttendanceRecords.GetAllAsync(r => r.StudentId == studentId);

            var result = records
                .GroupBy(r => r.CourseId)
                .Select(g =>
                {
                    var totalLectures = g.Select(r => r.LectureId).Distinct().Count();
                    var attendedCount = g.Count(r => r.IsAttended);
                    var percentage    = totalLectures > 0
                        ? Math.Round((double)attendedCount / totalLectures * 100, 2)
                        : 0.0;

                    return new
                    {
                        courseId      = g.Key,
                        totalLectures,
                        attendedCount,
                        percentage
                    };
                })
                .ToList();

            return Results.Ok(result);
        });

        // GET /api/v1/attendance/internal/students/{studentId}/overall
        group.MapGet("/students/{studentId:guid}/overall", async (
            Guid studentId,
            IUnitOfWork uow) =>
        {
            var records = await uow.AttendanceRecords.GetAllAsync(r => r.StudentId == studentId);

            if (!records.Any()) return Results.Ok(0.0);

            var overallPercentage = records
                .GroupBy(r => r.CourseId)
                .Select(g =>
                {
                    var totalLectures = g.Select(r => r.LectureId).Distinct().Count();
                    var attendedCount = g.Count(r => r.IsAttended);
                    return totalLectures > 0 ? (double)attendedCount / totalLectures * 100 : 0.0;
                })
                .Average();

            return Results.Ok(Math.Round(overallPercentage, 2));
        });

        // GET /api/v1/attendance/internal/courses/{courseId}
        group.MapGet("/courses/{courseId:guid}", async (
            Guid courseId,
            IUnitOfWork uow) =>
        {
            var records = await uow.AttendanceRecords.GetAllAsync(r => r.CourseId == courseId);

            var result = records
                .GroupBy(r => r.CourseId)
                .Select(g =>
                {
                    var totalLectures = g.Select(r => r.LectureId).Distinct().Count();
                    var attendedCount = g.Count(r => r.IsAttended);
                    var percentage    = totalLectures > 0
                        ? Math.Round((double)attendedCount / totalLectures * 100, 2)
                        : 0.0;

                    return new
                    {
                        courseId      = g.Key,
                        totalLectures,
                        attendedCount,
                        percentage
                    };
                })
                .ToList();

            return Results.Ok(result);
        });

        // GET /api/v1/attendance/internal/courses/{courseId}/average
        group.MapGet("/courses/{courseId:guid}/average", async (
            Guid courseId,
            IUnitOfWork uow) =>
        {
            var records = await uow.AttendanceRecords.GetAllAsync(r => r.CourseId == courseId);

            if (!records.Any()) return Results.Ok(0.0);

            var averagePercentage = records
                .GroupBy(r => r.StudentId)
                .Select(g =>
                {
                    var totalLectures = g.Select(r => r.LectureId).Distinct().Count();
                    var attendedCount = g.Count(r => r.IsAttended);
                    return totalLectures > 0 ? (double)attendedCount / totalLectures * 100 : 0.0;
                })
                .Average();

            return Results.Ok(Math.Round(averagePercentage, 2));
        });
    }
}
