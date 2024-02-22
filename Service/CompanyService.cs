using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges) 
    {
        var companies = repository.Company.GetAllCompanies(trackChanges);
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        
        logger.LogInfo($"All companies were returned successfully.");
        logger.LogInfo(companyDtos);
        
        return companyDtos;
    }
    
    public CompanyDto GetCompany(Guid companyId, bool trackChanges)
    {
        var company = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        return mapper.Map<CompanyDto>(company); 
    }

    public CompanyDto CreateCompany(CompanyForCreationDto companyForCreationDto)
    {
        var company = mapper.Map<Company>(companyForCreationDto); 
        
        repository.Company.CreateCompany(company); 
        repository.Save(); 
        
        return mapper.Map<CompanyDto>(company); 
    }

    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges) 
    { 
        if (companyIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var companies = repository.Company.GetByIds(companyIds, trackChanges); 
        
        if (companyIds.Count() != companies.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<CompanyDto>>(companies);  
    }

    public (IEnumerable<CompanyDto> companyDtos, string companyIds) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyForCreationDtos) 
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

        repository.Save(); 
        
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies); 
        var companyIds = string.Join(",", companyDtos.Select(company => company.Id)); 

        return (companyDtos, companyIds); 
    }

    public void DeleteCompany(Guid companyId, bool trackChanges) 
    {
        var company = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        
        repository.Company.DeleteCompany(company); 
        repository.Save(); 
    }

    public void UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdateDto, bool trackChanges) 
    { 
        var company = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        mapper.Map(companyForUpdateDto, company); 
        repository.Save(); 
    }
}