using AcademicService.Contracts;
using AcademicService.Data.Models;
using MediatR;

namespace AcademicService.Features.Departments;

public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, DepartmentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim();
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _unitOfWork.Departments.AnyAsync(d => d.Name == normalizedName))
            throw new InvalidOperationException($"Department name '{normalizedName}' already exists.");

        if (await _unitOfWork.Departments.AnyAsync(d => d.Code == normalizedCode))
            throw new InvalidOperationException($"Department code '{normalizedCode}' already exists.");

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Code = normalizedCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return new DepartmentResponse(department.Id, department.Name, department.Code);
    }
}
