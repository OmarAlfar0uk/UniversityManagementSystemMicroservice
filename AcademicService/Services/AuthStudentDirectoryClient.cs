using System.Net.Http.Headers;
using System.Net.Http.Json;
using AcademicService.Contracts;

namespace AcademicService.Services;

public class AuthStudentDirectoryClient : IStudentDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthStudentDirectoryClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<Guid>> GetStudentIdsByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var baseUrl = _configuration["ServiceEndpoints:AuthServiceBaseUrl"] ?? "http://localhost:5000";
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl.TrimEnd('/')}/api/v1/auth/admin/departments/{departmentId}/students");

        var authorization = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorization);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthEndpointResponse<List<DepartmentStudentDto>>>(
            cancellationToken: cancellationToken);

        return payload?.Data?.Select(student => student.StudentId).ToList() ?? [];
    }

    private sealed record AuthEndpointResponse<T>(
        T? Data,
        string Message,
        bool IsSuccess,
        int StatusCode,
        List<string> Errors,
        DateTime? Timestamp);

    private sealed record DepartmentStudentDto(
        Guid StudentId,
        string Email,
        string FullName,
        string UniversityId,
        Guid? DepartmentId);
}
