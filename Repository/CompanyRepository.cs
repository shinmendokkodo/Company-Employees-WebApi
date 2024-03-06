using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public async Task<PagedList<Company>> GetAllAsync(CompanyParameters companyParams, bool trackCompany)
    {
        var companies = await FindAll(trackCompany)
            .Search(companyParams.SearchTerm)
            .Sort(companyParams.OrderBy)
            .Skip((companyParams.PageNumber - 1) * companyParams.PageSize)
            .Take(companyParams.PageSize)
            .ToListAsync();

        var count = await FindAll(trackCompany).CountAsync();

        return new PagedList<Company>(companies, count, companyParams.PageNumber, companyParams.PageSize);
    }

    public async Task<Company?> GetByIdAsync(Guid companyId, bool trackCompany) =>
        await FindByCondition(c => c.Id.Equals(companyId), trackCompany)
            .SingleOrDefaultAsync();

    public new void Create(Company company) => base.Create(company);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackCompany) =>
        await FindByCondition(c => companyIds.Contains(c.Id), trackCompany)
            .ToListAsync();

    public new void Delete(Company company) => base.Delete(company);
}