using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
    {
        var companies = await FindAll(trackChanges)
        .OrderBy(company => company.Name)
        .Skip((companyParameters.PageNumber - 1) * companyParameters.PageSize)
        .Take(companyParameters.PageSize)
        .ToListAsync();

        var count = await FindAll(trackChanges).CountAsync();

        return new PagedList<Company>(companies, count, companyParameters.PageNumber, companyParameters.PageSize);
    }

    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges) => 
        await FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefaultAsync();
    
    public void CreateCompany(Company company) => Create(company);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackChanges) =>
        await FindByCondition(company => companyIds.Contains(company.Id), trackChanges)
        .ToListAsync();

    public void DeleteCompany(Company company) => Delete(company);
}