namespace AcademicService.Features.Lectures.GetLectureById;

public record LectureDetailsResponse(
    Guid Id,
    string Title,
    string ThumbnailUrl,
    bool HasPdf,
    bool HasVideo,
    bool HasAssignment
);
