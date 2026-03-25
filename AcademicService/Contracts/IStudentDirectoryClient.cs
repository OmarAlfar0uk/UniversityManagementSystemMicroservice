namespace AcademicService.Contracts;

public interface IStudentDirectoryClient
{
    Task<IReadOnlyList<Guid>> GetStudentIdsByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken);
}
