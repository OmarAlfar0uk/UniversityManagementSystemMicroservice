namespace AcademicService.Features.Lectures.GetLecturesByCourse;

public record LectureResponse(
    Guid Id,
    string Title,
    int OrderIndex,
    string ThumbnailUrl,
    Guid CourseId
);
