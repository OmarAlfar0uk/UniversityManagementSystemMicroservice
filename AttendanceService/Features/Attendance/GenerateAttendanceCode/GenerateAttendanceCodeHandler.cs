using AttendanceService.Contracts;
using AttendanceService.Data.Models;
using MediatR;

namespace AttendanceService.Features.Attendance.GenerateAttendanceCode;

public class GenerateAttendanceCodeHandler : IRequestHandler<GenerateAttendanceCodeCommand, AttendanceCodeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateAttendanceCodeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AttendanceCodeResponse> Handle(
        GenerateAttendanceCodeCommand request, CancellationToken cancellationToken)
    {
        // Deactivate existing active code for this lecture
        var existing = await _unitOfWork.AttendanceCodes.FindAsync(
            c => c.LectureId == request.LectureId && c.IsActive);
        if (existing is not null)
        {
            existing.IsActive = false;
            _unitOfWork.AttendanceCodes.Update(existing);
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(request.ExpiresInMinutes);
        var code = new AttendanceCode
        {
            Id = Guid.NewGuid(),
            LectureId = request.LectureId,
            CourseId = request.CourseId,
            Code = GenerateCode(),
            IsActive = true,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AttendanceCodes.AddAsync(code);
        await _unitOfWork.SaveChangesAsync();

        return new AttendanceCodeResponse(code.Code, code.ExpiresAt, code.LectureId);
    }

    private static string GenerateCode() =>
        Random.Shared.Next(100000, 999999).ToString();
}
