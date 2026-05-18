using AcademicService.Contracts;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.Assignments.CreateAssignment;

public class CreateAssignmentHandler : IRequestHandler<CreateAssignmentCommand, CreateAssignmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAssignmentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAssignmentResponse> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        if (lecture.CourseId != request.CourseId)
            throw new ArgumentException("Lecture does not belong to the selected course.");

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            LectureId = request.LectureId,
            CourseId = request.CourseId,
            Title = request.Title,
            Instructions = request.Instructions,
            Deadline = request.Deadline,
            IsOpen = request.IsOpen,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Assignments.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();

        return new CreateAssignmentResponse(
            assignment.Id,
            assignment.LectureId,
            assignment.CourseId,
            assignment.Title,
            assignment.Instructions,
            assignment.Deadline,
            assignment.IsOpen);
    }
}
