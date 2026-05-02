using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.GetLecturesByCourse;

public class GetLecturesByCourseHandler : IRequestHandler<GetLecturesByCourseQuery, IEnumerable<LectureResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public GetLecturesByCourseHandler(IUnitOfWork unitOfWork, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<LectureResponse>> Handle(
        GetLecturesByCourseQuery request,
        CancellationToken cancellationToken)
    {
        var lectures = await _unitOfWork.Lectures
            .GetAllAsync(l => l.CourseId == request.CourseId);

        return lectures
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LectureResponse(
                l.Id,
                l.Title,
                l.OrderIndex,
                _imageHelper.GetImageUrl(l.ThumbnailUrl ?? string.Empty) ?? string.Empty,
                l.CourseId));
    }
}
