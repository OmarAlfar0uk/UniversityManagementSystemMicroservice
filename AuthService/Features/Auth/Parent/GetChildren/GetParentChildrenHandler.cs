using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Features.Auth.Parent.GetChildren
{
    public class GetParentChildrenHandler : IRequestHandler<GetParentChildrenQuery, EndpointResponse<List<ParentChildDto>>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetParentChildrenHandler(
            UniversitySystemAuthContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<EndpointResponse<List<ParentChildDto>>> Handle(
            GetParentChildrenQuery request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Get parent id from token
            var parentIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(parentIdClaim))
            {
                return EndpointResponse<List<ParentChildDto>>
                    .UnauthorizedResponse("Unauthorized");
            }

            var parentId = Guid.Parse(parentIdClaim);

            var children = await _context.ParentStudents
              .AsNoTracking()
              .Where(x => x.ParentId == parentId)
              .Select(x => new ParentChildDto
              {
                  StudentId = x.Student.Id,
                  FullName = x.Student.FullName,
                  Email = x.Student.Email
              })
              .ToListAsync(cancellationToken);


            return EndpointResponse<List<ParentChildDto>>.SuccessResponse(
                children,
                "Children retrieved successfully"
            );
        }
    }
}
