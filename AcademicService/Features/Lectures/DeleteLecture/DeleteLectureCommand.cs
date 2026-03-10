using MediatR;

namespace AcademicService.Features.Lectures.DeleteLecture;

public record DeleteLectureCommand(Guid LectureId) : IRequest;
