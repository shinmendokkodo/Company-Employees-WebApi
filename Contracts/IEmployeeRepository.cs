using Entities;
using Shared.RequestFeatures;

namespace Contracts;

public interface IEmployeeRepository
{
    Task<PagedList<Employee>> GetEmployeesAsync(
        Guid companyId,
        EmployeeParameters employeeParameters,
        bool trackChanges
    );
    Task<Employee?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    Task<IEnumerable<Employee>> GetByIdsAsync(
        Guid companyId,
        IEnumerable<Guid> employeeIds,
        bool trackChanges
    );
    void DeleteEmployee(Employee employee);
}
