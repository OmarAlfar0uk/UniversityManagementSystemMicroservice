using MediatR;

namespace ProgressService.Features.Progress.SyncAttendance;

public record SyncAttendanceCommand(Guid LectureId, Guid CourseId, Guid StudentId) : IRequest<ProgressResponse>;
