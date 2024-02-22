using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public async Task<IEnumerable<Company>> GetAllCompanies(bool trackChanges) =>
        await FindAll(trackChanges)
        .OrderBy(company => company.Name)
        .ToListAsync();

    public async Task<Company?> GetCompany(Guid companyId, bool trackChanges) => 
        await FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefaultAsync();
    
    public void CreateCompany(Company company) => Create(company);

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges) =>
        await FindByCondition(company => companyIds.Contains(company.Id), trackChanges)
        .ToListAsync();

    public void DeleteCompany(Company company) => Delete(company);
}