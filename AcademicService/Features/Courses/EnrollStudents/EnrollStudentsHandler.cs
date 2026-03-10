using AcademicService.Contracts;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.Courses.EnrollStudents;

public class EnrollStudentsHandler : IRequestHandler<EnrollStudentsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollStudentsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EnrollStudentsCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        // Fetch existing enrollments to avoid duplicates
        var existing = await _unitOfWork.CourseEnrollments
            .GetAllAsync(e => e.CourseId == request.CourseId && request.StudentIds.Contains(e.StudentId));

        var existingIds = existing.Select(e => e.StudentId).ToHashSet();

        var newEnrollments = request.StudentIds
            .Where(sid => !existingIds.Contains(sid))
            .Select(sid => new CourseEnrollment
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                StudentId = sid,
                EnrolledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await _unitOfWork.CourseEnrollments.AddRangeAsync(newEnrollments);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
