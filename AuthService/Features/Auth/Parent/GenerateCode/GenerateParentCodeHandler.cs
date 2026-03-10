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

            var hasActiveCode = await _context.ParentCodes.AnyAsync(
                x => x.StudentId == studentId &&
                     !x.IsUsed &&
                     x.ExpiryDate > DateTime.UtcNow,
                cancellationToken);

            if (hasActiveCode)
            {
                return EndpointResponse<GenerateParentCodeResponse>.ErrorResponse(
                    "You already have an active parent code",
                    409
                );
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
