using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Lectures.GetLecturesByCourse;

public record GetLecturesByCourseQuery(Guid CourseId) : IRequest<IEnumerable<LectureResponse>>;
