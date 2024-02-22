using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetEmployees(Guid companyId, bool trackChanges) => 
        await FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges)
        .OrderBy(employee => employee.Name)
        .ToListAsync();
    
    public async Task<Employee?> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges) => 
        await FindByCondition(employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(employeeId), trackChanges)
        .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee) 
    { 
        employee.CompanyId = companyId; 
        Create(employee); 
    }

    public async Task<IEnumerable<Employee>> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges) =>
        await FindByCondition(employee => employee.CompanyId.Equals(companyId) && employeeIds.Contains(employee.Id), trackChanges)
        .ToListAsync();

    public void DeleteEmployee(Employee employee) => Delete(employee);
}