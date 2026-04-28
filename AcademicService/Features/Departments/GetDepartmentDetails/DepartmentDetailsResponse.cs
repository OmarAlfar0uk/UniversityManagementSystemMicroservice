namespace AcademicService.Features.Departments.GetDepartmentDetails;

public record DepartmentDetailsResponse(
    Guid Id,
    string Name,
    string Code,
    int StudentsCount,
    List<DepartmentCourseItem> Courses
);

public record DepartmentCourseItem(
    Guid Id,
    string Name,
    int EnrolledStudentsCount
);
