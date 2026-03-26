using System.Net.Http.Headers;
using System.Text.Json;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Services
{
    public class GradeServiceClient : IGradeServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GradeServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GradeServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GradeServiceClient> logger)
        {
            _client = factory.CreateClient("GradeService");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private void AttachHeaders()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return;

            var token = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();
            if (!string.IsNullOrEmpty(correlationId))
            {
                _client.DefaultRequestHeaders.Remove("X-Correlation-Id");
                _client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            }
        }

        public async Task<List<CourseGradeDto>> GetStudentGradesAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/grade/internal/students/{studentId}");
                if (!response.IsSuccessStatusCode) return new List<CourseGradeDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseGradeDto>>(json, _jsonOptions)
                       ?? new List<CourseGradeDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "GradeService: GetStudentGradesAsync failed for {StudentId}", studentId);
                return new List<CourseGradeDto>();
            }
        }

        public async Task<double?> GetStudentGpaAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/grade/internal/students/{studentId}/gpa");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<double?>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "GradeService: GetStudentGpaAsync failed for {StudentId}", studentId);
                return null;
            }
        }

        public async Task<List<StudentGradeDto>> GetCourseGradesAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/grade/internal/courses/{courseId}/grades");
                if (!response.IsSuccessStatusCode) return new List<StudentGradeDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<StudentGradeDto>>(json, _jsonOptions)
                       ?? new List<StudentGradeDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "GradeService: GetCourseGradesAsync failed for {CourseId}", courseId);
                return new List<StudentGradeDto>();
            }
        }

        public async Task<double> GetCourseAverageGradeAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/grade/internal/courses/{courseId}/average");
                if (!response.IsSuccessStatusCode) return 0.0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<double>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "GradeService: GetCourseAverageGradeAsync failed for {CourseId}", courseId);
                return 0.0;
            }
        }
    }
}
