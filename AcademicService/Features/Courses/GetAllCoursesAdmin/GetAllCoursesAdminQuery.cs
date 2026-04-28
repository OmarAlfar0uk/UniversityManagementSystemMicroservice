using MediatR;

namespace AcademicService.Features.Courses.GetAllCoursesAdmin;

public record GetAllCoursesAdminQuery : IRequest<GetAllCoursesAdminResponse>;
