using AcademicService.Features.Lectures.GetLecturesByCourse;
using MediatR;

namespace AcademicService.Features.Lectures.UpdateLecture;

public record UpdateLectureCommand(
    Guid LectureId,
    string Title,
    int OrderIndex,
    IFormFile? Thumbnail
) : IRequest<LectureResponse>;
