using AcademicService.Features.LectureMaterials.GetLecturePdf;
using MediatR;

namespace AcademicService.Features.LectureMaterials.UploadLecturePdf;

public record UploadLecturePdfCommand(Guid LectureId, IFormFile File) : IRequest<PdfResponse>;
