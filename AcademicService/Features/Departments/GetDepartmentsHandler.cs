using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.Departments;

public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsQuery, IReadOnlyList<DepartmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDepartmentsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DepartmentResponse>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();

        return departments
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentResponse(d.Id, d.Name, d.Code))
            .ToList();
    }
}
