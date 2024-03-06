namespace Shared.DataTransferObjects;

public record CompanyCreateDto : CompanyManipulateDto
{
    public IEnumerable<EmployeeCreateDto>? Employees { get; init; }
}