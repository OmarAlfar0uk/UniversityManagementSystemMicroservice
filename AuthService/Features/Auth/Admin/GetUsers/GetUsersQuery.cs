using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.GetUsers
{
    public class GetUsersQuery : IRequest<EndpointResponse<PaginatedResult<UserListItemDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
