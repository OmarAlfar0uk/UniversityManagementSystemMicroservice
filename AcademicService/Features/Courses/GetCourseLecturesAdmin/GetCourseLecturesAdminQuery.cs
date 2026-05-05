using MediatR;

namespace AcademicService.Features.Courses.GetCourseLecturesAdmin;

public record GetCourseLecturesAdminQuery(Guid CourseId) : IRequest<List<LectureAdminItem>>;
