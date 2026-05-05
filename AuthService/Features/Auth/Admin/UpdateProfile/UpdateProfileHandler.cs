using Auth.Contarcts;
using Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Admin.UpdateProfile;

public class UpdateProfileHandler(
    UserManager<ApplicationUser> userManager,
    IImageHelper imageHelper)
    : IRequestHandler<UpdateProfileCommand, AdminProfileResponse>
{
    public async Task<AdminProfileResponse> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.AdminId.ToString())
            ?? throw new KeyNotFoundException($"Admin with ID {request.AdminId} not found.");

        // Update FullName — split into FirstName / LastName (space-separated)
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var parts = request.FullName.Trim().Split(' ', 2);
            user.FirstName = parts[0];
            user.LastName  = parts.Length > 1 ? parts[1] : string.Empty;
        }

        // Update PhoneNumber (only if provided)
        if (request.PhoneNumber is not null)
            user.PhoneNumber = request.PhoneNumber;

        // Update Profile Image (only if provided)
        if (request.ProfileImage is not null)
        {
            // Delete old image from disk if one exists
            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                imageHelper.DeleteImage(user.ProfileImageUrl);

            // Save new image and store relative path in DB
            user.ProfileImageUrl = await imageHelper.SaveImageAsync(
                request.ProfileImage, "Profiles");
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update profile: {errors}");
        }

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? "Admin";

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
