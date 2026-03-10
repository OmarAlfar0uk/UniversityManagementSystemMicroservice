using AttendanceService.Contracts;
using MediatR;

namespace AttendanceService.Features.Attendance.GetCoursesAttendance;

public class GetCoursesAttendanceHandler : IRequestHandler<GetCoursesAttendanceQuery, IEnumerable<CourseAttendanceResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCoursesAttendanceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CourseAttendanceResponse>> Handle(
        GetCoursesAttendanceQuery request, CancellationToken cancellationToken)
    {
        var records = await _unitOfWork.AttendanceRecords
            .GetAllAsync(r => r.StudentId == request.StudentId);

        return records
            .GroupBy(r => r.CourseId)
            .Select(g =>
            {
                var attended = g.Count(r => r.IsAttended);
                var total = g.Count();
                return new CourseAttendanceResponse(
                    g.Key,
                    total,
                    attended,
                    total == 0 ? 0m : Math.Round((decimal)attended / total * 100, 2)
                );
            });
    }
}
