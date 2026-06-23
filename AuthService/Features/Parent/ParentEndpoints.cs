using Auth_Service.Features.Shared;
using AuthService.Features.Parent.GetChildProfile;
using AuthService.Features.Parent.GetChildSchedule;
using AuthService.Features.Parent.GetChildCourses;
using AuthService.Features.Parent.GetChildGrades;
using MediatR;
using System.Security.Claims;

namespace AuthService.Features.Parent
{
    /// <summary>
    /// Parent → Child data endpoints.
    /// Base route: /api/v1/auth/parent
    /// All endpoints require the "Parent" role.
    /// </summary>
    public static class ParentEndpoints
    {
        public static IEndpointRouteBuilder MapParentChildEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/auth/parent")
                           .RequireAuthorization()
                           .RequireAuthorization(p => p.RequireRole("Parent"))
                           .WithTags("Parent – Child Data");

            // GET /api/v1/auth/parent/child/profile
            group.MapGet("/child/profile", async (ISender sender, ClaimsPrincipal user) =>
            {
                var parentId = ResolveParentId(user);
                var result   = await sender.Send(new GetChildProfileQuery(parentId));
                return result.ToHttpResult();
            })
            .WithSummary("Get child's profile")
            .WithDescription("Returns the linked student's basic profile information.");

            // GET /api/v1/auth/parent/child/schedule
            group.MapGet("/child/schedule", async (ISender sender, ClaimsPrincipal user) =>
            {
                var parentId = ResolveParentId(user);
                var result   = await sender.Send(new GetChildScheduleQuery(parentId));
                return result.ToHttpResult();
            })
            .WithSummary("Get child's class and midterm schedule")
            .WithDescription("Returns schedule image URLs from AcademicService for the student's department.");

            // GET /api/v1/auth/parent/child/courses
            group.MapGet("/child/courses", async (ISender sender, ClaimsPrincipal user) =>
            {
                var parentId = ResolveParentId(user);
                var result   = await sender.Send(new GetChildCoursesQuery(parentId));
                return result.ToHttpResult();
            })
            .WithSummary("Get child's enrolled courses with scores")
            .WithDescription("Returns enrolled courses merged with grade data from GradeService.");

            // GET /api/v1/auth/parent/child/grades
            group.MapGet("/child/grades", async (ISender sender, ClaimsPrincipal user) =>
            {
                var parentId = ResolveParentId(user);
                var result   = await sender.Send(new GetChildGradesQuery(parentId));
                return result.ToHttpResult();
            })
            .WithSummary("Get child's grades and GPA")
            .WithDescription("Returns all course grades, GPA, and grade letters for the linked student.");

            return app;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static Guid ResolveParentId(ClaimsPrincipal user)
        {
            var value = user.FindFirstValue("id")
                     ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException("Parent identity claim not found.");

            return Guid.Parse(value);
        }
    }
}

