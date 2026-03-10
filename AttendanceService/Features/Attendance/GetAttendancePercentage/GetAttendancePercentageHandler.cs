using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetAttendancePercentage;

public class GetAttendancePercentageHandler : IRequestHandler<GetAttendancePercentageQuery, AttendancePercentageResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAttendancePercentageHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AttendancePercentageResponse> Handle(
        GetAttendancePercentageQuery request, CancellationToken cancellationToken)
    {
        var records = await _unitOfWork.AttendanceRecords
            .GetAllAsync(r => r.CourseId == request.CourseId && r.StudentId == request.StudentId);

        var total = records.Count();
        var attended = records.Count(r => r.IsAttended);
        var percentage = total == 0 ? 0m : Math.Round((decimal)attended / total * 100, 2);

        return new AttendancePercentageResponse(request.CourseId, percentage);
    }
}
