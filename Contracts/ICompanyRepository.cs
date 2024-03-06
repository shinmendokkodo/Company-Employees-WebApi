using Entities;
using Shared.RequestFeatures;

namespace Contracts;

public interface ICompanyRepository
{
    Task<PagedList<Company>> GetAllAsync(CompanyParameters companyParams, bool trackCompany);
    Task<Company?> GetByIdAsync(Guid companyId, bool trackCompany);
    void Create(Company company);
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackCompany);
    void Delete(Company company);
}