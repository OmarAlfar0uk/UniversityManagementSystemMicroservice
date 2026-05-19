using System.Net.Http.Headers;
using System.Text.Json;
using AcademicService.Contracts;
using AcademicService.Dtos;
using Microsoft.Extensions.Logging;

namespace AcademicService.Services
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
                var userInfo = DeserializeUserInfo(json);
                if (userInfo is null) return null;

                return new UserInfoDto
                {
                    Id = userInfo.Id == Guid.Empty ? userId : userInfo.Id,
                    FirstName = string.IsNullOrWhiteSpace(userInfo.FirstName)
                        ? GetFirstName(userInfo.FullName)
                        : userInfo.FirstName,
                    FullName = userInfo.FullName ?? string.Empty,
                    Email = userInfo.Email ?? string.Empty,
                    Role = userInfo.Role ?? string.Empty,
                    ProfileImageUrl = userInfo.ProfileImageUrl,
                    Department = userInfo.Department,
                    DepartmentId = userInfo.DepartmentId
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AuthService: GetUserInfoAsync failed for {UserId}", userId);
                return null;
            }
        }

        private static UserInfoDto? DeserializeUserInfo(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("data", out var data) &&
                data.ValueKind == JsonValueKind.Object)
            {
                return data.Deserialize<UserInfoDto>(_jsonOptions);
            }

            return root.Deserialize<UserInfoDto>(_jsonOptions);
        }

        private static string GetFirstName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            return fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
