using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.GetLecturesByCourse;

public class GetLecturesByCourseHandler : IRequestHandler<GetLecturesByCourseQuery, IEnumerable<LectureResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLecturesByCourseHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<LectureResponse>> Handle(
        GetLecturesByCourseQuery request,
        CancellationToken cancellationToken)
    {
        var lectures = await _unitOfWork.Lectures
            .GetAllAsync(l => l.CourseId == request.CourseId);

        return lectures
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LectureResponse(l.Id, l.Title, l.OrderIndex, l.ThumbnailUrl ?? string.Empty, l.CourseId));
    }
}
