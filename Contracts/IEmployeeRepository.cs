using Entities;

namespace Contracts;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployees(Guid companyId, bool trackChanges);
    Task<Employee?> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    Task<IEnumerable<Employee>> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges);
    void DeleteEmployee(Employee employee);
}