using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetLecturesAttendance;

public class GetLecturesAttendanceHandler : IRequestHandler<GetLecturesAttendanceQuery, IEnumerable<LectureAttendanceResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLecturesAttendanceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<LectureAttendanceResponse>> Handle(
        GetLecturesAttendanceQuery request, CancellationToken cancellationToken)
    {
        var records = await _unitOfWork.AttendanceRecords
            .GetAllAsync(r => r.CourseId == request.CourseId && r.StudentId == request.StudentId);

        return records.Select(r => new LectureAttendanceResponse(
            r.LectureId,
            r.LectureTitle ?? string.Empty,
            r.IsAttended,
            r.RegisteredAt
        ));
    }
}
