using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetCourseLecturesAdmin;

public class GetCourseLecturesAdminHandler : IRequestHandler<GetCourseLecturesAdminQuery, List<LectureAdminItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseLecturesAdminHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LectureAdminItem>> Handle(
        GetCourseLecturesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var lectures = await _unitOfWork.Lectures.GetAllAsync(l => l.CourseId == request.CourseId);
        var result = new List<LectureAdminItem>();

        foreach (var lecture in lectures)
        {
            var hasPdf        = await _unitOfWork.LecturePdfs.AnyAsync(p => p.LectureId == lecture.Id);
            var hasVideo      = await _unitOfWork.LectureVideos.AnyAsync(v => v.LectureId == lecture.Id);
            var hasAssignment = await _unitOfWork.Assignments.AnyAsync(a => a.LectureId == lecture.Id);

            result.Add(new LectureAdminItem(
                lecture.Id,
                lecture.Title,
                lecture.OrderIndex,
                hasPdf,
                hasVideo,
                hasAssignment
            ));
        }

        return result;
    }
}
