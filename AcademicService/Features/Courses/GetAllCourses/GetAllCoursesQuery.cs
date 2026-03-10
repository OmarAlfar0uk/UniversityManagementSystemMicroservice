using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Courses.GetAllCourses;

public record GetAllCoursesQuery(
    Guid StudentId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResponse<CourseResponse>>;
