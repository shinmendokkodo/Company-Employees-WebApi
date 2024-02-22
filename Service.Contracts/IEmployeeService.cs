using Entities;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetEmployees(Guid companyId, bool trackChanges);
    Task<EmployeeDto> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges);
    Task<IEnumerable<EmployeeDto>> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges);
    Task DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges);
    Task UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges);
    Task<(EmployeeForUpdateDto employeeForUpdateDto, Employee employee)> GetEmployeeForPatch(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges);
    Task SaveChangesForPatch(EmployeeForUpdateDto employeeForUpdateDto, Employee employee);
}