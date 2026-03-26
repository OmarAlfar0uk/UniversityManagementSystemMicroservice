using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Student.GetProfile;

public class GetProfileHandler(
    UserManager<ApplicationUser> userManager,
    IImageHelper imageHelper)
    : IRequestHandler<GetProfileQuery, StudentProfileResponse>
{
    public async Task<StudentProfileResponse> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.StudentId.ToString())
            ?? throw new KeyNotFoundException($"Student with ID {request.StudentId} not found.");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        return new StudentProfileResponse(
            Id:              user.Id,
            FullName:        user.FullName,
            Email:           user.Email ?? string.Empty,
            UniversityId:    user.UniversityId,
            DepartmentId:    user.DepartmentId,
            Role:            role,
            PhoneNumber:     user.PhoneNumber,
            ProfileImageUrl: string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                             ? null
                             : imageHelper.GetImageUrl(user.ProfileImageUrl)
        );
    }
}
