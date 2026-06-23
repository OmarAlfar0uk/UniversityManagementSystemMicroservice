using Auth_Service.Features.Shared;
using AuthService.Contracts;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Features.Auth.Parent.GenerateCode
{
    public class GenerateParentCodeHandler
        : IRequestHandler<GenerateParentCodeCommand, EndpointResponse<GenerateParentCodeResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthAuditLogger _auditLogger;

        public GenerateParentCodeHandler(
            UniversitySystemAuthContext context,
            IHttpContextAccessor httpContextAccessor , IAuthAuditLogger auditLogger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogger = auditLogger;
        }

        public async Task<EndpointResponse<GenerateParentCodeResponse>> Handle(
            GenerateParentCodeCommand request,
            CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return EndpointResponse<GenerateParentCodeResponse>
                    .UnauthorizedResponse("Unauthorized");
            }

            var studentId = Guid.Parse(userIdClaim);

            // Invalidate all existing active (unused & non-expired) codes for this student
            var existingActiveCodes = await _context.ParentCodes
                .Where(x => x.StudentId == studentId && !x.IsUsed && x.ExpiryDate > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var oldCode in existingActiveCodes)
            {
                oldCode.IsUsed = true;
            }

            var code = Random.Shared.Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddHours(48);

            var parentCode = new ParentCode
            {
                Id = Guid.NewGuid(),
                Code = code,
                StudentId = studentId,
                ExpiryDate = expiry,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ParentCodes.Add(parentCode);
            await _context.SaveChangesAsync(cancellationToken);
            await _auditLogger.LogAsync(
            action: "GenerateParentCode",
            description: "Parent code generated"
               );

            return EndpointResponse<GenerateParentCodeResponse>.SuccessResponse(
                new GenerateParentCodeResponse
                {
                    Code = code,
                    ExpiryDate = expiry
                },
                "Parent code generated successfully"
            );
        }
    }
}
