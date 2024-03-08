using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IEmployeeRepository
{
    Task<PagedList<Employee>> GetAllAsync(Guid companyId, EmployeeParameters employeeParams, bool trackEmployee);
    Task<Employee?> GetByIdAsync(Guid companyId, Guid employeeId, bool trackEmployee);
    void Create(Guid companyId, Employee employee);
    Task<IEnumerable<Employee>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackEmployee);
    void Delete(Employee employee);
}