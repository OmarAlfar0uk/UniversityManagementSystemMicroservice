using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AuthService.Features.Parent.GetChildCourses
{
    public class GetChildCoursesHandler
        : IRequestHandler<GetChildCoursesQuery, EndpointResponse<ChildCoursesResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GetChildCoursesHandler(
            UniversitySystemAuthContext context,
            UserManager<ApplicationUser> userManager,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _context             = context;
            _userManager         = userManager;
            _httpClientFactory   = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<EndpointResponse<ChildCoursesResponse>> Handle(
            GetChildCoursesQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Resolve StudentId
            var link = await _context.ParentStudents
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.ParentId == request.ParentId, cancellationToken);

            if (link is null)
            {
                return EndpointResponse<ChildCoursesResponse>.NotFoundResponse(
                    "No child linked to this parent account.");
            }

            var student = await _userManager.FindByIdAsync(link.StudentId.ToString());
            if (student is null)
            {
                return EndpointResponse<ChildCoursesResponse>.NotFoundResponse(
                    "Student account not found.");
            }

            var token = GetBearerToken();

            // 2. Fetch courses and grades in parallel
            var (courses, gradeMap) = await FetchCoursesAndGradesAsync(
                link.StudentId, token, cancellationToken);

            // 3. Combine
            var courseItems = courses.Select(c =>
            {
                gradeMap.TryGetValue(c.CourseId, out var grade);
                var total = grade?.TotalScore;
                return new ChildCourseItem(
                    CourseId:         c.CourseId,
                    CourseName:       c.CourseName,
                    CoverImageUrl:    c.CoverImageUrl,
                    DoctorId:         c.DoctorId,
                    MidtermScore:     grade?.MidtermScore,
                    FinalScore:       grade?.FinalScore,
                    TotalScore:       total,
                    PerformanceLabel: GetPerformanceLabel(total)
                );
            }).ToList();

            var response = new ChildCoursesResponse(
                StudentId:   student.Id,
                StudentName: student.FullName,
                Courses:     courseItems
            );

            return EndpointResponse<ChildCoursesResponse>.SuccessResponse(
                response, "Child courses retrieved successfully.");
        }

        // ── Parallel fetching ─────────────────────────────────────────────────

        private async Task<(List<AcademicCourseDto> Courses, Dictionary<Guid, GradeDto> GradeMap)>
            FetchCoursesAndGradesAsync(Guid studentId, string? token, CancellationToken ct)
        {
            var academicClient = _httpClientFactory.CreateClient("AcademicService");
            var gradeClient    = _httpClientFactory.CreateClient("GradeService");
            AttachToken(academicClient, token);
            AttachToken(gradeClient,    token);

            var coursesTask = FetchCoursesAsync(academicClient, studentId, ct);
            var gradesTask  = FetchGradesAsync(gradeClient,    studentId, ct);

            await Task.WhenAll(coursesTask, gradesTask);

            var courses  = await coursesTask;
            var gradeMap = (await gradesTask)
                .ToDictionary(g => g.CourseId, g => g);

            return (courses, gradeMap);
        }

        private static async Task<List<AcademicCourseDto>> FetchCoursesAsync(
            HttpClient client, Guid studentId, CancellationToken ct)
        {
            try
            {
                var resp = await client.GetAsync(
                    $"/api/v1/academic/internal/students/{studentId}/courses", ct);

                if (!resp.IsSuccessStatusCode) return [];

                var json   = await resp.Content.ReadAsStringAsync(ct);
                var parsed = JsonSerializer.Deserialize<AcademicCoursesWrapper>(json, _jsonOptions);
                return parsed?.Data ?? [];
            }
            catch { return []; }
        }

        private static async Task<List<GradeDto>> FetchGradesAsync(
            HttpClient client, Guid studentId, CancellationToken ct)
        {
            try
            {
                var resp = await client.GetAsync(
                    $"/api/v1/grade/internal/students/{studentId}", ct);

                if (!resp.IsSuccessStatusCode) return [];

                var json   = await resp.Content.ReadAsStringAsync(ct);
                var parsed = JsonSerializer.Deserialize<GradesWrapper>(json, _jsonOptions);
                return parsed?.Data ?? [];
            }
            catch { return []; }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private string? GetBearerToken() =>
            _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

        private static void AttachToken(HttpClient client, string? token)
        {
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer", token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetPerformanceLabel(decimal? total) => total switch
        {
            >= 85 => "Excellent",
            >= 70 => "Good",
            >= 50 => "At Risk",
            _     => "No Data"
        };

        // ── Internal DTOs ─────────────────────────────────────────────────────

        private sealed class AcademicCoursesWrapper
        {
            public List<AcademicCourseDto>? Data { get; set; }
        }

        private sealed class AcademicCourseDto
        {
            public Guid    CourseId      { get; set; }
            public string  CourseName    { get; set; } = string.Empty;
            public string? CoverImageUrl { get; set; }
            public Guid    DoctorId      { get; set; }
        }

        private sealed class GradesWrapper
        {
            public List<GradeDto>? Data { get; set; }
        }

        private sealed class GradeDto
        {
            public Guid     CourseId      { get; set; }
            public decimal? MidtermScore  { get; set; }
            public decimal? FinalScore    { get; set; }
            public decimal? TotalScore    { get; set; }
        }
    }
}
