using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.DeleteLecture;

public class DeleteLectureHandler : IRequestHandler<DeleteLectureCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLectureHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        _unitOfWork.Lectures.Remove(lecture);
        await _unitOfWork.SaveChangesAsync();
    }
}
