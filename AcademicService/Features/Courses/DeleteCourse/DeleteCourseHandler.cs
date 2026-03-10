using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.DeleteCourse;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {request.CourseId} not found.");

        _unitOfWork.Courses.Remove(course);
        await _unitOfWork.SaveChangesAsync();
    }
}
