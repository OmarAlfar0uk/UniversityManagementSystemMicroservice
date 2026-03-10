using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetAttendanceStatus;

public class GetAttendanceStatusHandler : IRequestHandler<GetAttendanceStatusQuery, AttendanceStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAttendanceStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AttendanceStatusResponse> Handle(GetAttendanceStatusQuery request, CancellationToken cancellationToken)
    {
        var record = await _unitOfWork.AttendanceRecords.FindAsync(
            r => r.LectureId == request.LectureId && r.StudentId == request.StudentId);

        return new AttendanceStatusResponse(request.LectureId, record?.IsAttended ?? false);
    }
}
