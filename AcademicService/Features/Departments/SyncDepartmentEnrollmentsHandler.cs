using AcademicService.Contracts;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.Departments;

public class SyncDepartmentEnrollmentsHandler
    : IRequestHandler<SyncDepartmentEnrollmentsCommand, SyncDepartmentEnrollmentsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentDirectoryClient _studentDirectoryClient;

    public SyncDepartmentEnrollmentsHandler(
        IUnitOfWork unitOfWork,
        IStudentDirectoryClient studentDirectoryClient)
    {
        _unitOfWork = unitOfWork;
        _studentDirectoryClient = studentDirectoryClient;
    }

    public async Task<SyncDepartmentEnrollmentsResponse> Handle(
        SyncDepartmentEnrollmentsCommand request,
        CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
        if (department is null)
            throw new KeyNotFoundException($"Department {request.DepartmentId} not found.");

        var studentIds = (await _studentDirectoryClient
            .GetStudentIdsByDepartmentAsync(request.DepartmentId, cancellationToken))
            .Distinct()
            .ToList();

        var courseOfferings = (await _unitOfWork.Courses
            .GetAllAsync(course => course.DepartmentId == request.DepartmentId && course.IsActive))
            .ToList();

        if (studentIds.Count == 0 || courseOfferings.Count == 0)
        {
            return new SyncDepartmentEnrollmentsResponse(
                request.DepartmentId,
                studentIds.Count,
                courseOfferings.Count,
                0);
        }

        var courseIds = courseOfferings.Select(course => course.Id).ToList();
        var existingEnrollments = (await _unitOfWork.CourseEnrollments
            .GetAllAsync(enrollment =>
                courseIds.Contains(enrollment.CourseId) &&
                studentIds.Contains(enrollment.StudentId)))
            .ToList();

        var existingPairs = existingEnrollments
            .Select(enrollment => (enrollment.CourseId, enrollment.StudentId))
            .ToHashSet();

        var newEnrollments = new List<CourseEnrollment>();
        foreach (var course in courseOfferings)
        {
            foreach (var studentId in studentIds)
            {
                if (existingPairs.Contains((course.Id, studentId)))
                    continue;

                newEnrollments.Add(new CourseEnrollment
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    StudentId = studentId,
                    EnrolledAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (newEnrollments.Count > 0)
        {
            await _unitOfWork.CourseEnrollments.AddRangeAsync(newEnrollments);
            await _unitOfWork.SaveChangesAsync();
        }

        return new SyncDepartmentEnrollmentsResponse(
            request.DepartmentId,
            studentIds.Count,
            courseOfferings.Count,
            newEnrollments.Count);
    }
}
