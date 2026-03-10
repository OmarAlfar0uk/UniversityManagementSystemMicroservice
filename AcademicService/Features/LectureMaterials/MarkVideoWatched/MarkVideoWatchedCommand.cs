using MediatR;

namespace AcademicService.Features.LectureMaterials.MarkVideoWatched;

public record MarkVideoWatchedCommand(Guid LectureId, Guid StudentId) : IRequest;
