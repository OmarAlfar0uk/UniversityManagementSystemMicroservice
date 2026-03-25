using AcademicService.Features.Courses.GetAllCourses;
using MediatR;

namespace AcademicService.Features.CourseCatalogs;

public record CreateCourseOfferingCommand(
    Guid CourseCatalogId,
    Guid DepartmentId,
    Guid DoctorId) : IRequest<CourseResponse>;
