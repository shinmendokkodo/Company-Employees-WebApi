using Contracts;
using Entities;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        [.. FindAll(trackChanges).OrderBy(company => company.Name)];
    
    public Company? GetCompany(Guid companyId, bool trackChanges) => 
        FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();
    
    public void CreateCompany(Company company) => Create(company);

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges) => 
        [.. FindByCondition(company => companyIds.Contains(company.Id), trackChanges)];

    public void DeleteCompany(Company company) => Delete(company);
}