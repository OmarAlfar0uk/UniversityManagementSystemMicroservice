using System.Net.Http.Headers;
using System.Text.Json;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Services
{
    public class AcademicServiceClient : IAcademicServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AcademicServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AcademicServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AcademicServiceClient> logger)
        {
            _client = factory.CreateClient("AcademicService");
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

        public async Task<List<CourseDto>> GetEnrolledCoursesAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/academic/internal/students/{studentId}/courses");
                if (!response.IsSuccessStatusCode) return new List<CourseDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseDto>>(json, _jsonOptions)
                       ?? new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AcademicService: GetEnrolledCoursesAsync failed for {StudentId}", studentId);
                return new List<CourseDto>();
            }
        }

        public async Task<List<CourseDto>> GetDoctorCoursesAsync(Guid doctorId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/academic/internal/doctors/{doctorId}/courses");
                if (!response.IsSuccessStatusCode) return new List<CourseDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseDto>>(json, _jsonOptions)
                       ?? new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AcademicService: GetDoctorCoursesAsync failed for {DoctorId}", doctorId);
                return new List<CourseDto>();
            }
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync("/api/v1/academic/internal/courses");
                if (!response.IsSuccessStatusCode) return new List<CourseDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CourseDto>>(json, _jsonOptions)
                       ?? new List<CourseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AcademicService: GetAllCoursesAsync failed");
                return new List<CourseDto>();
            }
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync("/api/v1/academic/internal/courses/count");
                if (!response.IsSuccessStatusCode) return 0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AcademicService: GetTotalCoursesCountAsync failed");
                return 0;
            }
        }

        public async Task<List<LectureDto>> GetCourseLecturesAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/academic/internal/courses/{courseId}/lectures");
                if (!response.IsSuccessStatusCode) return new List<LectureDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<LectureDto>>(json, _jsonOptions)
                       ?? new List<LectureDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AcademicService: GetCourseLecturesAsync failed for {CourseId}", courseId);
                return new List<LectureDto>();
            }
        }

        public async Task<int> GetCourseEnrollmentCountAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/academic/internal/courses/{courseId}/enrollment-count");
                if (!response.IsSuccessStatusCode) return 0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AcademicService: GetCourseEnrollmentCountAsync failed for {CourseId}", courseId);
                return 0;
            }
        }
    }
}
