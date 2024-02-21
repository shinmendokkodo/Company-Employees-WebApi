using Contracts;
using Entities;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    
}