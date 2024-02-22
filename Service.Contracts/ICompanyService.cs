using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompanies(bool trackChanges);
    Task<CompanyDto> GetCompany(Guid companyId, bool trackChanges);
    Task<CompanyDto> CreateCompany(CompanyForCreationDto company);
    Task<IEnumerable<CompanyDto>> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges);
    Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection);
    Task DeleteCompany(Guid companyId, bool trackChanges);
    Task UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdateDto, bool trackChanges);
}