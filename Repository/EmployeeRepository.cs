using Contracts;
using Entities;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
        [.. FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges).OrderBy(employee => employee.Name)];
    
    public Employee? GetEmployee(Guid companyId, Guid employeeId, bool trackChanges) => 
        FindByCondition(employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(employeeId), trackChanges)
        .SingleOrDefault();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee) 
    { 
        employee.CompanyId = companyId; 
        Create(employee); 
    }

    public IEnumerable<Employee> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges) =>
        [.. FindByCondition(employee => employee.CompanyId.Equals(companyId) && employeeIds.Contains(employee.Id), trackChanges)];

    public void DeleteEmployee(Employee employee) => Delete(employee);
}