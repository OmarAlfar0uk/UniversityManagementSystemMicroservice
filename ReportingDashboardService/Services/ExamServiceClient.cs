using System.Net.Http.Headers;
using System.Text.Json;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Services
{
    public class ExamServiceClient : IExamServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ExamServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ExamServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ExamServiceClient> logger)
        {
            _client = factory.CreateClient("ExamService");
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

        public async Task<List<QuizResultDto>> GetStudentQuizResultsAsync(Guid studentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/exam/internal/students/{studentId}/quiz-results");
                if (!response.IsSuccessStatusCode) return new List<QuizResultDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<QuizResultDto>>(json, _jsonOptions)
                       ?? new List<QuizResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "ExamService: GetStudentQuizResultsAsync failed for {StudentId}", studentId);
                return new List<QuizResultDto>();
            }
        }

        public async Task<int> GetPendingEssaysCountAsync(Guid doctorId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/exam/internal/doctors/{doctorId}/pending-essays/count");
                if (!response.IsSuccessStatusCode) return 0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "ExamService: GetPendingEssaysCountAsync failed for {DoctorId}", doctorId);
                return 0;
            }
        }

        public async Task<List<QuizStatsDto>> GetCourseQuizStatsAsync(Guid courseId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/exam/internal/courses/{courseId}/quiz-stats");
                if (!response.IsSuccessStatusCode) return new List<QuizStatsDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<QuizStatsDto>>(json, _jsonOptions)
                       ?? new List<QuizStatsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "ExamService: GetCourseQuizStatsAsync failed for {CourseId}", courseId);
                return new List<QuizStatsDto>();
            }
        }
    }
}
