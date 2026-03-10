using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLectureVideo;

public record GetLectureVideoQuery(Guid LectureId) : IRequest<VideoResponse>;
