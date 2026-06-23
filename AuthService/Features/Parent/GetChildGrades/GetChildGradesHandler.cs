using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AuthService.Features.Parent.GetChildGrades
{
    public class GetChildGradesHandler
        : IRequestHandler<GetChildGradesQuery, EndpointResponse<ChildGradesResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GetChildGradesHandler(
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

        public async Task<EndpointResponse<ChildGradesResponse>> Handle(
            GetChildGradesQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Resolve StudentId
            var link = await _context.ParentStudents
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.ParentId == request.ParentId, cancellationToken);

            if (link is null)
            {
                return EndpointResponse<ChildGradesResponse>.NotFoundResponse(
                    "No child linked to this parent account.");
            }

            var student = await _userManager.FindByIdAsync(link.StudentId.ToString());
            if (student is null)
            {
                return EndpointResponse<ChildGradesResponse>.NotFoundResponse(
                    "Student account not found.");
            }

            var token       = GetBearerToken();
            var gradeClient = _httpClientFactory.CreateClient("GradeService");
            var acadClient  = _httpClientFactory.CreateClient("AcademicService");
            AttachToken(gradeClient, token);
            AttachToken(acadClient,  token);

            // 2. Fetch grades + GPA + course names in parallel
            var gradesTask      = FetchGradesAsync(gradeClient, link.StudentId, cancellationToken);
            var gpaTask         = FetchGpaAsync(gradeClient,    link.StudentId, cancellationToken);
            var courseNamesTask = FetchCourseNamesAsync(acadClient, link.StudentId, cancellationToken);

            await Task.WhenAll(gradesTask, gpaTask, courseNamesTask);

            var grades      = await gradesTask;
            var gpa         = await gpaTask;
            var courseNames = await courseNamesTask;

            // 3. Build grade items
            var gradeItems = grades.Select(g =>
            {
                courseNames.TryGetValue(g.CourseId, out var courseName);
                return new CourseGradeItem(
                    CourseId:     g.CourseId,
                    CourseName:   courseName,
                    MidtermScore: g.MidtermScore,
                    FinalScore:   g.FinalScore,
                    TotalScore:   g.TotalScore,
                    GradeLetter:  GetGradeLetter(g.TotalScore)
                );
            }).ToList();

            var response = new ChildGradesResponse(
                StudentId:   student.Id,
                StudentName: student.FullName,
                GPA:         gpa,
                GpaLabel:    GetGpaLabel(gpa),
                Grades:      gradeItems
            );

            return EndpointResponse<ChildGradesResponse>.SuccessResponse(
                response, "Child grades retrieved successfully.");
        }

        // ── Fetchers ──────────────────────────────────────────────────────────

        private static async Task<List<GradeDto>> FetchGradesAsync(
            HttpClient client, Guid studentId, CancellationToken ct)
        {
            try
            {
                var resp = await client.GetAsync(
                    $"/api/v1/grade/internal/students/{studentId}", ct);

                if (!resp.IsSuccessStatusCode) return [];

                var json = await resp.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<List<GradeDto>>(json, _jsonOptions) ?? [];
            }
            catch { return []; }
        }

        private static async Task<double?> FetchGpaAsync(
            HttpClient client, Guid studentId, CancellationToken ct)
        {
            try
            {
                var resp = await client.GetAsync(
                    $"/api/v1/grade/internal/students/{studentId}/gpa", ct);

                if (!resp.IsSuccessStatusCode) return null;

                var json = await resp.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<double?>(json, _jsonOptions);
            }
            catch { return null; }
        }

        private static async Task<Dictionary<Guid, string>> FetchCourseNamesAsync(
            HttpClient client, Guid studentId, CancellationToken ct)
        {
            try
            {
                var resp = await client.GetAsync(
                    $"/api/v1/academic/internal/students/{studentId}/courses", ct);

                if (!resp.IsSuccessStatusCode) return [];

                var json = await resp.Content.ReadAsStringAsync(ct);
                var list = JsonSerializer.Deserialize<List<AcademicCourseDto>>(json, _jsonOptions) ?? [];
                return list.ToDictionary(c => c.Id, c => c.Name);
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

        private static string GetGpaLabel(double? gpa) => gpa switch
        {
            >= 3.7 => "Excellent",
            >= 3.0 => "Very Good",
            >= 2.0 => "Good",
            >= 1.0 => "Pass",
            _      => "Fail"
        };

        private static string GetGradeLetter(decimal? score) => score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            < 60  => "F",
            _     => "N/A"
        };

        // ── Internal DTOs ─────────────────────────────────────────────────────

        private sealed class GradeDto
        {
            public Guid     CourseId     { get; set; }
            public decimal? MidtermScore { get; set; }
            public decimal? FinalScore   { get; set; }
            public decimal? TotalScore   { get; set; }
        }

        private sealed class AcademicCourseDto
        {
            public Guid   Id   { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
