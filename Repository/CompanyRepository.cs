using Contracts;
using Entities;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    
}