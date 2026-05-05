using Auth.Contarcts;
using Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Doctor.GetProfile;

public class GetProfileHandler(
    UserManager<ApplicationUser> userManager,
    IImageHelper imageHelper)
    : IRequestHandler<GetProfileQuery, DoctorProfileResponse>
{
    public async Task<DoctorProfileResponse> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.DoctorId.ToString())
            ?? throw new KeyNotFoundException($"Doctor with ID {request.DoctorId} not found.");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Doctor";

        return new DoctorProfileResponse(
            Id:              user.Id,
            FullName:        user.FullName,
            Email:           user.Email ?? string.Empty,
            ProfileImageUrl: string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                             ? null
                             : imageHelper.GetImageUrl(user.ProfileImageUrl),
            PhoneNumber:     user.PhoneNumber,
            Role:            role,
            DepartmentId:    user.DepartmentId
        );
    }
}
