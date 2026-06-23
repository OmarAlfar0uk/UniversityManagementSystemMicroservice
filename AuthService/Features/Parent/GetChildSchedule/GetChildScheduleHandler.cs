using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AuthService.Features.Parent.GetChildSchedule
{
    public class GetChildScheduleHandler
        : IRequestHandler<GetChildScheduleQuery, EndpointResponse<ChildScheduleResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GetChildScheduleHandler(
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

        public async Task<EndpointResponse<ChildScheduleResponse>> Handle(
            GetChildScheduleQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Resolve StudentId
            var link = await _context.ParentStudents
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.ParentId == request.ParentId, cancellationToken);

            if (link is null)
            {
                return EndpointResponse<ChildScheduleResponse>.NotFoundResponse(
                    "No child linked to this parent account.");
            }

            // 2. Load student for DepartmentId + name
            var student = await _userManager.FindByIdAsync(link.StudentId.ToString());
            if (student is null)
            {
                return EndpointResponse<ChildScheduleResponse>.NotFoundResponse(
                    "Student account not found.");
            }

            // 3. Call AcademicService for schedule
            var schedules = new List<ScheduleItem>();
            try
            {
                var client = _httpClientFactory.CreateClient("AcademicService");
                AttachToken(client);

                var url = student.DepartmentId.HasValue
                    ? $"/api/v1/academic/internal/schedule?departmentId={student.DepartmentId}"
                    : $"/api/v1/academic/schedule";

                var httpResponse = await client.GetAsync(url, cancellationToken);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    var parsed = JsonSerializer.Deserialize<AcademicScheduleWrapper>(json, _jsonOptions);
                    if (parsed?.Data is not null)
                    {
                        schedules = parsed.Data
                            .Select(s => new ScheduleItem(
                                Id:           s.Id,
                                ImageUrl:     s.ImageUrl ?? string.Empty,
                                Type:         s.Type ?? "ClassSchedule",
                                AcademicYear: s.AcademicYear))
                            .ToList();
                    }
                }
            }
            catch
            {
                // Graceful degradation — return empty schedule list
            }

            var response = new ChildScheduleResponse(
                StudentId:    student.Id,
                StudentName:  student.FullName,
                DepartmentId: student.DepartmentId,
                Schedules:    schedules
            );

            return EndpointResponse<ChildScheduleResponse>.SuccessResponse(
                response, "Child schedule retrieved successfully.");
        }

        private void AttachToken(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer", token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase));
            }
        }

        // ── Internal DTOs for deserializing AcademicService response ─────────

        private sealed class AcademicScheduleWrapper
        {
            public List<AcademicScheduleDto>? Data { get; set; }
        }

        private sealed class AcademicScheduleDto
        {
            public Guid    Id           { get; set; }
            public string? ImageUrl     { get; set; }
            public string? Type         { get; set; }
            public string? AcademicYear { get; set; }
        }
    }
}
