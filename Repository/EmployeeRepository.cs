using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges) => 
        await FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges)
        .OrderBy(employee => employee.Name)
        .ToListAsync();
    
    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges) => 
        await FindByCondition(employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(employeeId), trackChanges)
        .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee) 
    { 
        employee.CompanyId = companyId; 
        Create(employee); 
    }

    public async Task<IEnumerable<Employee>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges) =>
        await FindByCondition(employee => employee.CompanyId.Equals(companyId) && employeeIds.Contains(employee.Id), trackChanges)
        .ToListAsync();

    public void DeleteEmployee(Employee employee) => Delete(employee);
}