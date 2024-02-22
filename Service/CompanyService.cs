using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public async Task<IEnumerable<CompanyDto>> GetAllCompanies(bool trackChanges) 
    {
        var companies = await repository.Company.GetAllCompanies(trackChanges);
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        
        logger.LogInfo($"All companies were returned successfully.");
        logger.LogInfo(companyDtos);
        
        return companyDtos;
    }
    
    public async Task<CompanyDto> GetCompany(Guid companyId, bool trackChanges)
    {
        var company = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        return mapper.Map<CompanyDto>(company); 
    }

    public async Task<CompanyDto> CreateCompany(CompanyForCreationDto companyForCreationDto)
    {
        var company = mapper.Map<Company>(companyForCreationDto); 
        
        repository.Company.CreateCompany(company); 
        await repository.SaveAsync(); 
        
        return mapper.Map<CompanyDto>(company); 
    }

    public async Task<IEnumerable<CompanyDto>> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges) 
    { 
        if (companyIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var companies = await repository.Company.GetByIds(companyIds, trackChanges); 
        
        if (companyIds.Count() != companies.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<CompanyDto>>(companies);  
    }

    public async Task<(IEnumerable<CompanyDto> companyDtos, string companyIds)> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyForCreationDtos) 
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

    public async Task DeleteCompany(Guid companyId, bool trackChanges) 
    {
        var company = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        
        repository.Company.DeleteCompany(company); 
        await repository.SaveAsync(); 
    }

    public async Task UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdateDto, bool trackChanges) 
    { 
        var company = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        mapper.Map(companyForUpdateDto, company); 
        await repository.SaveAsync(); 
    }
}