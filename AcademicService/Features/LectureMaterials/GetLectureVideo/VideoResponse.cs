namespace AcademicService.Features.LectureMaterials.GetLectureVideo;

public record VideoResponse(Guid Id, string VideoUrl, int DurationInMinutes, Guid LectureId);
