using MediatR;

namespace ProgressService.Features.Progress.UpdateProgress;

public record UpdateProgressCommand(Guid LectureId, Guid CourseId, Guid StudentId, int TotalLecturesInCourse) : IRequest;
