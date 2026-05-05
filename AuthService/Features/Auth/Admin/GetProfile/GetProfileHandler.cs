using Auth.Contarcts;
using Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Admin.GetProfile;

public class GetProfileHandler(
    UserManager<ApplicationUser> userManager,
    IImageHelper imageHelper)
    : IRequestHandler<GetProfileQuery, AdminProfileResponse>
{
    public async Task<AdminProfileResponse> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.AdminId.ToString())
            ?? throw new KeyNotFoundException($"Admin with ID {request.AdminId} not found.");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Admin";

        return new AdminProfileResponse(
            Id:              user.Id,
            FullName:        user.FullName,
            Email:           user.Email ?? string.Empty,
            ProfileImageUrl: string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                             ? null
                             : imageHelper.GetImageUrl(user.ProfileImageUrl),
            PhoneNumber:     user.PhoneNumber,
            Role:            role
        );
    }
}
