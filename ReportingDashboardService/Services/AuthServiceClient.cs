using System.Net.Http.Headers;
using System.Text.Json;
using ReportingDashboardService.Contracts;
using ReportingDashboardService.Dtos;

namespace ReportingDashboardService.Services
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthServiceClient> logger)
        {
            _client = factory.CreateClient("AuthService");
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

        public async Task<UserInfoDto?> GetUserInfoAsync(Guid userId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync($"/api/v1/auth/internal/users/{userId}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserInfoDto>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AuthService: GetUserInfoAsync failed for {UserId}", userId);
                return null;
            }
        }

        public async Task<List<StudentDto>> GetStudentsByDepartmentAsync(Guid departmentId)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/auth/internal/departments/{departmentId}/students");
                if (!response.IsSuccessStatusCode) return new List<StudentDto>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<StudentDto>>(json, _jsonOptions)
                       ?? new List<StudentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AuthService: GetStudentsByDepartmentAsync failed for {DeptId}", departmentId);
                return new List<StudentDto>();
            }
        }

        public async Task<int> GetTotalUsersCountAsync(string role)
        {
            try
            {
                AttachHeaders();
                var response = await _client.GetAsync(
                    $"/api/v1/auth/internal/users/count?role={role}");
                if (!response.IsSuccessStatusCode) return 0;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "AuthService: GetTotalUsersCountAsync failed for role {Role}", role);
                return 0;
            }
        }
    }
}
