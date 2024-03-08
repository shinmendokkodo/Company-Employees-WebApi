using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface ICompanyService
{
    Task<(LinkResponse linkResponse, Metadata metadata)> GetAllAsync(CompanyLinkParameters linkParams, bool trackCompany = false);

    Task<CompanyDto> GetByIdAsync(Guid companyId, bool trackChanges = false);

    Task<CompanyDto> CreateAsync(CompanyCreateDto company);

    Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackChanges = false);

    Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCollectionAsync(IEnumerable<CompanyCreateDto> companyCreateDtos);

    Task DeleteAsync(Guid companyId, bool trackChanges = false);

    Task UpdateAsync(Guid companyId, CompanyUpdateDto companyUpdateDto, bool trackChanges = true);

    Task<(CompanyManipulateDto companyManipulateDto, Company company)> GetByIdPatchAsync(Guid companyId, bool trackCompany = true);

    Task SavePatchAsync(CompanyManipulateDto companyManipulateDto, Company company);
}