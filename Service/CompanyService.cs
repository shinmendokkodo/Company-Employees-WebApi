using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public async Task<(IEnumerable<CompanyDto> companyDtos, Metadata metadata)> GetAllAsync(CompanyParameters companyParams, bool trackCompany)
    {
        var companies = await repository.Company.GetAllAsync(companyParams, trackCompany);
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);

        logger.LogInfo("All companies were returned successfully.");
        logger.LogInfo(companyDtos);

        return (companyDtos, companies.Metadata);
    }

    public async Task<CompanyDto> GetByIdAsync(Guid companyId, bool trackCompany)
    {
        var company = await ReturnCompanyIfExistsAsync(companyId, trackCompany);
        return mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> CreateAsync(CompanyCreateDto companyCreateDto)
    {
        var company = mapper.Map<Company>(companyCreateDto);

        repository.Company.Create(company);
        await repository.SaveAsync();

        return mapper.Map<CompanyDto>(company);
    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackCompany)
    {
        if (companyIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var companies = await repository.Company.GetByIdsAsync(companyIds, trackCompany);

        if (companyIds.Count() != companies.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCollectionAsync(IEnumerable<CompanyCreateDto> companyCreateDtos)
    {
        if (companyCreateDtos is null)
        {
            throw new CompanyCollectionBadRequestException();
        }

        var companies = mapper.Map<IEnumerable<Company>>(companyCreateDtos);
        foreach (var company in companies)
        {
            repository.Company.Create(company);
        }

        await repository.SaveAsync();

        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        var companyIds = string.Join(",", companyDtos.Select(company => company.Id));

        return (companyDtos, companyIds);
    }

    public async Task DeleteAsync(Guid companyId, bool trackCompany)
    {
        var company = await ReturnCompanyIfExistsAsync(companyId, trackCompany);
        repository.Company.Delete(company);
        await repository.SaveAsync();
    }

    public async Task<(CompanyManipulateDto companyManipulateDto, Company company)> GetByIdPatchAsync(Guid companyId, bool trackCompany)
    {
        var company = await ReturnCompanyIfExistsAsync(companyId, trackCompany);
        var companyManipulateDto = mapper.Map<CompanyManipulateDto>(company);
        return (companyManipulateDto, company);
    }

    public async Task SavePatchAsync(CompanyManipulateDto companyManipulateDto, Company company)
    {
        mapper.Map(companyManipulateDto, company);
        await repository.SaveAsync();
    }

    public async Task UpdateAsync(Guid companyId, CompanyUpdateDto companyUpdateDto, bool trackCompany)
    {
        var company = await ReturnCompanyIfExistsAsync(companyId, trackCompany);
        mapper.Map(companyUpdateDto, company);
        await repository.SaveAsync();
    }

    private async Task<Company> ReturnCompanyIfExistsAsync(Guid id, bool trackCompany) =>
        await repository.Company.GetByIdAsync(id, trackCompany) ?? 
            throw new CompanyNotFoundException(id);
}