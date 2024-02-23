using Entities;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface ICompanyService
{
    Task<(IEnumerable<CompanyDto> companyDtos, MetaData metaData)> GetAllCompaniesAsync(
        CompanyParameters companyParameters,
        bool trackChanges
    );

    Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges);

    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);

    Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackChanges);

    Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCompanyCollectionAsync(
        IEnumerable<CompanyForCreationDto> companyForCreationDtos
    );

    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);

    Task UpdateCompanyAsync(
        Guid companyId,
        CompanyForUpdateDto companyForUpdateDto,
        bool trackChanges
    );

    Task<(
        CompanyForManipulationDto companyForManipulationDto,
        Company company
    )> GetCompanyForPatchAsync(Guid companyId, bool trackChanges);
    
    Task SaveChangesForPatchAsync(
        CompanyForManipulationDto companyForManipulationDto,
        Company company
    );
}
