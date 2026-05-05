namespace AcademicService.Features.Courses.GetCourseLecturesAdmin;

public record LectureAdminItem(
    Guid Id,
    string Title,
    int OrderIndex,
    bool HasPdf,
    bool HasVideo,
    bool HasAssignment
);
