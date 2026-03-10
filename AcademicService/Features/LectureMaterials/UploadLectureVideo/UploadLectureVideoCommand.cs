using AcademicService.Features.LectureMaterials.GetLectureVideo;
using MediatR;

namespace AcademicService.Features.LectureMaterials.UploadLectureVideo;

public record UploadLectureVideoCommand(Guid LectureId, IFormFile VideoFile) : IRequest<VideoResponse>;
