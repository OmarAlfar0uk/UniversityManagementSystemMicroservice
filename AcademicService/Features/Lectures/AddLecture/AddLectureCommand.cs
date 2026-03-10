using AcademicService.Features.Lectures.GetLecturesByCourse;
using MediatR;

namespace AcademicService.Features.Lectures.AddLecture;

public record AddLectureCommand(
    Guid CourseId,
    string Title,
    int OrderIndex,
    IFormFile? Thumbnail
) : IRequest<LectureResponse>;
