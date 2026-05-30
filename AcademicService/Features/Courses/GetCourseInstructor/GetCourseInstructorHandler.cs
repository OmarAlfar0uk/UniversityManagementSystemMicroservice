using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseInstructor;

public class GetCourseInstructorHandler : IRequestHandler<GetCourseInstructorQuery, InstructorResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthServiceClient _authServiceClient;

    public GetCourseInstructorHandler(
        IUnitOfWork unitOfWork,
        IAuthServiceClient authServiceClient)
    {
        _unitOfWork = unitOfWork;
        _authServiceClient = authServiceClient;
    }

    public async Task<InstructorResponse> Handle(
        GetCourseInstructorQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        var userInfo = await _authServiceClient.GetUserInfoAsync(course.DoctorId);

        return new InstructorResponse(
            DoctorId: course.DoctorId,
            CourseId: course.Id,
            FullName: NullIfWhiteSpace(userInfo?.FullName),
            Email: NullIfWhiteSpace(userInfo?.Email),
            ProfileImageUrl: NullIfWhiteSpace(userInfo?.ProfileImageUrl)
        );
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
