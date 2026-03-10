using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetLectureAttendees;

public class GetLectureAttendeesHandler : IRequestHandler<GetLectureAttendeesQuery, IEnumerable<AttendeeResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLectureAttendeesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AttendeeResponse>> Handle(GetLectureAttendeesQuery request, CancellationToken cancellationToken)
    {
        var records = await _unitOfWork.AttendanceRecords
            .GetAllAsync(r => r.LectureId == request.LectureId && r.IsAttended);

        return records.Select(r => new AttendeeResponse(r.StudentId, r.RegisteredAt, r.IsManual));
    }
}
