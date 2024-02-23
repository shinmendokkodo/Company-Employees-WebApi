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
    public async Task<(IEnumerable<CompanyDto> companyDtos, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges) 
    {
        var companies = await repository.Company.GetAllCompaniesAsync(companyParameters, trackChanges);
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        
        logger.LogInfo($"All companies were returned successfully.");
        logger.LogInfo(companyDtos);
        
        return (companyDtos, companies.MetaData);
    }
    
    public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
        return mapper.Map<CompanyDto>(company); 
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto companyForCreationDto)
    {
        var company = mapper.Map<Company>(companyForCreationDto); 
        
        repository.Company.CreateCompany(company); 
        await repository.SaveAsync(); 
        
        return mapper.Map<CompanyDto>(company); 
    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> companyIds, bool trackChanges) 
    { 
        if (companyIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var companies = await repository.Company.GetByIdsAsync(companyIds, trackChanges); 
        
        if (companyIds.Count() != companies.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<CompanyDto>>(companies);  
    }

    public async Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyForCreationDtos) 
    {
        if (companyForCreationDtos is null)
        {
            throw new CompanyCollectionBadRequest();
        }

        var companies = mapper.Map<IEnumerable<Company>>(companyForCreationDtos); 
        foreach (var company in companies) 
        { 
            repository.Company.CreateCompany(company); 
        }

        await repository.SaveAsync(); 
        
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies); 
        var companyIds = string.Join(",", companyDtos.Select(company => company.Id)); 

        return (companyDtos, companyIds); 
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges) 
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        repository.Company.DeleteCompany(company); 
        await repository.SaveAsync(); 
    }

    public async Task<(CompanyForManipulationDto companyForManipulationDto, Company company)> GetCompanyForPatchAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var companyForManipulationDto = mapper.Map<CompanyForManipulationDto>(company);
        return (companyForManipulationDto, company);
    }

    public async Task SaveChangesForPatchAsync(CompanyForManipulationDto companyForManipulationDto, Company company)
    {
        mapper.Map(companyForManipulationDto, company);
        await repository.SaveAsync();
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdateDto, bool trackChanges) 
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
        mapper.Map(companyForUpdateDto, company); 
        await repository.SaveAsync(); 
    }

    private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges) => 
        await repository.Company.GetCompanyAsync(id, trackChanges) 
        ?? throw new CompanyNotFoundException(id);
}