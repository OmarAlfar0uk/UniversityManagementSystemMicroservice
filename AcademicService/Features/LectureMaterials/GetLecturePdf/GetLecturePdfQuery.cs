using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLecturePdf;

public record GetLecturePdfQuery(Guid LectureId) : IRequest<PdfResponse>;
