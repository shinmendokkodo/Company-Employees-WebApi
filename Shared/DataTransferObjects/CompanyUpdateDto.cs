namespace Shared.DataTransferObjects;

public record CompanyUpdateDto : CompanyManipulateDto
{
    public IEnumerable<EmployeeCreateDto>? Employees { get; init; }
}