using AcademicService.Contracts;
using AcademicService.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AcademicService.Features.Courses.GetCourseInstructor;

public class GetCourseInstructorHandler : IRequestHandler<GetCourseInstructorQuery, InstructorResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly ILogger<GetCourseInstructorHandler> _logger;

    public GetCourseInstructorHandler(
        IUnitOfWork unitOfWork,
        IAuthServiceClient authServiceClient,
        ILogger<GetCourseInstructorHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _authServiceClient = authServiceClient;
        _logger = logger;
    }

    public async Task<InstructorResponse> Handle(
        GetCourseInstructorQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        string firstName = course.DoctorFirstName ?? string.Empty;
        string fullName = course.DoctorFullName ?? string.Empty;
        string? profileImage = null;
        string? department = null;

        var userInfo = await _authServiceClient.GetUserInfoAsync(course.DoctorId);
        if (userInfo is not null)
        {
            firstName = userInfo.FirstName ?? string.Empty;
            fullName = userInfo.FullName ?? string.Empty;
            profileImage = userInfo.ProfileImageUrl;
            department = userInfo.Department;
        }

        return new InstructorResponse(
            course.DoctorId,
            course.Id,
            firstName ?? string.Empty,
            fullName,
            profileImage,
            department
        );
    }
}
