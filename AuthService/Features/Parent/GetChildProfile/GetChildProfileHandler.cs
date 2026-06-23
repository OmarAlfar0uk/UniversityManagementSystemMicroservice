using Auth.Contarcts;
using Auth.Models;
using Auth_Service.Features.Shared;
using AuthService.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Parent.GetChildProfile
{
    public class GetChildProfileHandler
        : IRequestHandler<GetChildProfileQuery, EndpointResponse<ChildProfileResponse>>
    {
        private readonly UniversitySystemAuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageHelper _imageHelper;

        public GetChildProfileHandler(
            UniversitySystemAuthContext context,
            UserManager<ApplicationUser> userManager,
            IImageHelper imageHelper)
        {
            _context    = context;
            _userManager = userManager;
            _imageHelper = imageHelper;
        }

        public async Task<EndpointResponse<ChildProfileResponse>> Handle(
            GetChildProfileQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Resolve StudentId from ParentStudent link
            var link = await _context.ParentStudents
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.ParentId == request.ParentId, cancellationToken);

            if (link is null)
            {
                return EndpointResponse<ChildProfileResponse>.NotFoundResponse(
                    "No child linked to this parent account.");
            }

            // 2. Load student via UserManager
            var student = await _userManager.FindByIdAsync(link.StudentId.ToString());
            if (student is null)
            {
                return EndpointResponse<ChildProfileResponse>.NotFoundResponse(
                    "Student account not found.");
            }

            // 3. Build response
            var profileImageUrl = string.IsNullOrEmpty(student.ProfileImageUrl)
                ? null
                : _imageHelper.GetImageUrl(student.ProfileImageUrl);

            var response = new ChildProfileResponse(
                StudentId:       student.Id,
                FullName:        student.FullName,
                Email:           student.Email,
                UniversityId:    student.UniversityId,
                DepartmentId:    student.DepartmentId,
                ProfileImageUrl: profileImageUrl
            );

            return EndpointResponse<ChildProfileResponse>.SuccessResponse(
                response, "Child profile retrieved successfully.");
        }
    }
}
