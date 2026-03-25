using Auth.Models;
using Auth_Service.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Features.Auth.Admin.GetStudentsByDepartment;

public class GetStudentsByDepartmentHandler
    : IRequestHandler<GetStudentsByDepartmentQuery, EndpointResponse<IReadOnlyList<DepartmentStudentDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetStudentsByDepartmentHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<EndpointResponse<IReadOnlyList<DepartmentStudentDto>>> Handle(
        GetStudentsByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");

        var result = students
            .Where(student => student.DepartmentId == request.DepartmentId)
            .OrderBy(student => student.FirstName)
            .ThenBy(student => student.LastName)
            .Select(student => new DepartmentStudentDto(
                student.Id,
                student.Email ?? string.Empty,
                student.FullName,
                student.UniversityId,
                student.DepartmentId))
            .ToList()
            .AsReadOnly();

        return EndpointResponse<IReadOnlyList<DepartmentStudentDto>>.SuccessResponse(
            result,
            "Department students retrieved successfully");
    }
}
