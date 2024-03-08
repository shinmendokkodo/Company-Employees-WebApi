using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public async Task<PagedList<Employee>> GetAllAsync(Guid companyId, EmployeeParameters employeeParams, bool trackEmployee)
    {
        var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackEmployee)
            .Filter(employeeParams.MinAge, employeeParams.MaxAge)
            .Search(employeeParams.SearchTerm)
            .Sort(employeeParams.OrderBy)
            .Skip((employeeParams.PageNumber - 1) * employeeParams.PageSize)
            .Take(employeeParams.PageSize)
            .ToListAsync();

        var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackEmployee)
            .CountAsync();

        return new PagedList<Employee>(employees, count, employeeParams.PageNumber, employeeParams.PageSize);
    }

    public async Task<Employee?> GetByIdAsync(Guid companyId, Guid employeeId, bool trackEmployee) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackEmployee)
            .SingleOrDefaultAsync();

    public void Create(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public async Task<IEnumerable<Employee>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackEmployee) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && employeeIds.Contains(e.Id), trackEmployee)
            .ToListAsync();

    public new void Delete(Employee employee) => base.Delete(employee);
}