using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.GetLectureById;

public record GetLectureByIdQuery(Guid CourseId, Guid LectureId) : IRequest<LectureDetailsResponse>;
