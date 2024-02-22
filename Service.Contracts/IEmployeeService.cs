using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges);
    IEnumerable<EmployeeDto> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges);
    void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges);
}