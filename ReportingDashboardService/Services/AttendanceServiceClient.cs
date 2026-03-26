using System.Net.Http.Headers;
using System.Text.Json;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Services
{
    public class AttendanceServiceClient : IAttendanceServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AttendanceServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AttendanceServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AttendanceServiceClient> logger)
        {
            _client = factory.CreateClient("AttendanceService");
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

        public async Task<List<CourseAttendanceDto>> GetStudentAttendanceAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/attendance/internal/students/{studentId}");
                if (!response.IsSuccessStatusCode) return new List<CourseAttendanceDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseAttendanceDto>>(json, _jsonOptions)
                       ?? new List<CourseAttendanceDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AttendanceService: GetStudentAttendanceAsync failed for {StudentId}", studentId);
                return new List<CourseAttendanceDto>();
            }
        }

        public async Task<double> GetStudentOverallAttendancePercentageAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/attendance/internal/students/{studentId}/overall");
                if (!response.IsSuccessStatusCode) return 0.0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<double>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AttendanceService: GetStudentOverallAttendancePercentageAsync failed for {StudentId}",
                    studentId);
                return 0.0;
            }
        }

        public async Task<List<CourseAttendanceDto>> GetCourseAttendanceAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/attendance/internal/courses/{courseId}");
                if (!response.IsSuccessStatusCode) return new List<CourseAttendanceDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseAttendanceDto>>(json, _jsonOptions)
                       ?? new List<CourseAttendanceDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AttendanceService: GetCourseAttendanceAsync failed for {CourseId}", courseId);
                return new List<CourseAttendanceDto>();
            }
        }

        public async Task<double> GetCourseAttendanceAverageAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/attendance/internal/courses/{courseId}/average");
                if (!response.IsSuccessStatusCode) return 0.0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<double>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AttendanceService: GetCourseAttendanceAverageAsync failed for {CourseId}",
                    courseId);
                return 0.0;
            }
        }
    }
}
