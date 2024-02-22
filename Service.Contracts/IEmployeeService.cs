using Entities;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges);
    IEnumerable<EmployeeDto> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges);
    void DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges);
    void UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges);
    (EmployeeForUpdateDto employeeForUpdateDto, Employee employee) GetEmployeeForPatch(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges); 
    void SaveChangesForPatch(EmployeeForUpdateDto employeeForUpdateDto, Employee employee);
}