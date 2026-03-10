namespace AcademicService.Features.LectureMaterials.GetLecturePdf;

public record PdfResponse(Guid Id, string FileUrl, Guid LectureId);
