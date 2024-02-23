using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext)
    : RepositoryBase<Employee>(repositoryContext),
        IEmployeeRepository
{
    public async Task<PagedList<Employee>> GetEmployeesAsync(
        Guid companyId,
        EmployeeParameters employeeParameters,
        bool trackChanges
    )
    {
        var employees = await FindByCondition(
                employee =>
                    employee.CompanyId.Equals(companyId)
                    && employee.Age >= employeeParameters.MinAge
                    && employee.Age <= employeeParameters.MaxAge,
                trackChanges
            )
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
            .OrderBy(employee => employee.Name)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync();

        var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .CountAsync();

        return new PagedList<Employee>(
            employees,
            count,
            employeeParameters.PageNumber,
            employeeParameters.PageSize
        );
    }

    public async Task<Employee?> GetEmployeeAsync(
        Guid companyId,
        Guid employeeId,
        bool trackChanges
    ) =>
        await FindByCondition(
                employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(employeeId),
                trackChanges
            )
            .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public async Task<IEnumerable<Employee>> GetByIdsAsync(
        Guid companyId,
        IEnumerable<Guid> employeeIds,
        bool trackChanges
    ) =>
        await FindByCondition(
                employee =>
                    employee.CompanyId.Equals(companyId) && employeeIds.Contains(employee.Id),
                trackChanges
            )
            .ToListAsync();

    public void DeleteEmployee(Employee employee) => Delete(employee);
}
