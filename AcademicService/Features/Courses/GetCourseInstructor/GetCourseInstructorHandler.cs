using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseInstructor;

public class GetCourseInstructorHandler : IRequestHandler<GetCourseInstructorQuery, InstructorResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseInstructorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InstructorResponse> Handle(
        GetCourseInstructorQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        return new InstructorResponse(course.DoctorId, course.Id);
    }
}
