using System.Net.Http.Headers;
using AuthService.Contracts;

namespace AuthService.Services
{
    public class AcademicServiceClient : IAcademicServiceClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AcademicServiceClient> _logger;

        public AcademicServiceClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AcademicServiceClient> logger)
        {
            _client = factory.CreateClient("AcademicService");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> DeleteStudentEnrollmentsAsync(
            Guid studentId,
            CancellationToken cancellationToken)
        {
            try
            {
                AttachHeaders();

                var response = await _client.DeleteAsync(
                    $"/api/v1/academic/internal/students/{studentId}/enrollments",
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                    return true;

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "AcademicService cleanup failed for student {StudentId}. Status: {StatusCode}. Body: {Body}",
                    studentId,
                    (int)response.StatusCode,
                    responseBody);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "AcademicService cleanup threw an exception for student {StudentId}",
                    studentId);

                return false;
            }
        }

        private void AttachHeaders()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null)
                return;

            var token = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                _client.DefaultRequestHeaders.Remove("X-Correlation-Id");
                _client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            }
        }
    }
}
