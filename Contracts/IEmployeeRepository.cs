using Entities;

namespace Contracts;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
    Employee? GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    IEnumerable<Employee> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges);
}